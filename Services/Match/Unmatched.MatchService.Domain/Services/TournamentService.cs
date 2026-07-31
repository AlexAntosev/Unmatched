namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Communication.Player;
using Unmatched.MatchService.Domain.Communication.Player.Dto;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.Domain.Tournaments;

public class TournamentService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICatalogHeroCache catalogHeroCache,
    ICatalogMapCache catalogMapCache,
    IPlayerCache playerCache,
    ITournamentFormatGeneratorFactory generatorFactory,
    TournamentAwardScheduler awardScheduler,
    TournamentTitleAwarder titleAwarder) : ITournamentService
{
    public async Task<Tournament> AddAsync(Tournament dto)
    {
        var tournament = mapper.Map<TournamentEntity>(dto);
        tournament.Status = TournamentStatus.Draft;
        tournament.CurrentStage = tournament.InitialStage;

        tournament.Participants = dto.ParticipantHeroIds
            .Select(heroId => new TournamentParticipantEntity { HeroId = heroId })
            .ToList();

        // Champion is mandatory regardless of what the create request asked for.
        tournament.TournamentTitles = dto.TitleKinds
            .Append(TournamentTitleKind.Champion)
            .Distinct()
            .Select(kind => new TournamentTitleEntity { Kind = kind })
            .ToList();

        var created = await unitOfWork.Tournaments.AddAsync(tournament);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<Tournament>(created);
    }

    public async Task<IEnumerable<Tournament>> GetAsync()
    {
        var entities = await unitOfWork.Tournaments.GetAsync();
        var tournaments = mapper.Map<IEnumerable<Tournament>>(entities);

        return tournaments;
    }

    public async Task<Tournament> GetAsync(Guid id)
    {
        var entity = await unitOfWork.Tournaments.GetByIdWithParticipantsAsync(id);
        var tournament = mapper.Map<Tournament>(entity);

        return tournament;
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Tournaments.Delete(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<Tournament?> UpdateImageAsync(Guid id, string imageFileName)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(id);
        if (tournament is null)
        {
            return null;
        }

        tournament.ImageFileName = imageFileName;
        await unitOfWork.Tournaments.AddOrUpdateAsync(tournament);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<Tournament>(tournament);
    }

    public async Task<Tournament?> UpdateTrophyImageAsync(Guid id, string imageFileName)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(id);
        if (tournament is null)
        {
            return null;
        }

        tournament.TrophyImageFileName = imageFileName;
        await unitOfWork.Tournaments.AddOrUpdateAsync(tournament);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<Tournament>(tournament);
    }

    public async Task<IEnumerable<TournamentStanding>> GetStandingsAsync(Guid tournamentId)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdWithParticipantsAsync(tournamentId);
        if (tournament is null)
        {
            throw new KeyNotFoundException($"No tournament with key {tournamentId}");
        }

        var standingsByHero = tournament.Participants.ToDictionary(
            p => p.HeroId,
            p => new TournamentStanding { HeroId = p.HeroId, FinalPlacement = p.FinalPlacement });

        var finishedMatches = (await unitOfWork.Matches.GetByTournamentAsync(tournamentId)).Where(m => !m.IsPlanned);
        foreach (var fighter in finishedMatches.SelectMany(m => m.Fighters))
        {
            if (!standingsByHero.TryGetValue(fighter.HeroId, out var standing))
            {
                continue;
            }

            if (fighter.IsWinner)
            {
                standing.Wins++;
            }
            else
            {
                standing.Losses++;
            }
        }

        return standingsByHero.Values.OrderByDescending(s => s.Wins).ThenBy(s => s.Losses);
    }

    public async Task<Tournament> CompleteAsync(Guid tournamentId)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdWithParticipantsAsync(tournamentId);
        if (tournament is null)
        {
            throw new KeyNotFoundException($"No tournament with key {tournamentId}");
        }

        if (tournament.Status == TournamentStatus.Completed)
        {
            throw new InvalidOperationException("This tournament is already completed.");
        }

        var matches = await unitOfWork.Matches.GetByTournamentAsync(tournamentId);
        if (matches.Any(m => m.IsPlanned))
        {
            throw new InvalidOperationException("Cannot complete this tournament: some matches are still planned.");
        }

        var completedAt = DateTime.UtcNow;
        var schedule = awardScheduler.Schedule(tournament, matches, completedAt);

        foreach (var award in schedule.Awards)
        {
            await unitOfWork.TournamentAwards.AddAsync(award);
        }

        foreach (var participant in tournament.Participants)
        {
            if (schedule.FinalPlacements.TryGetValue(participant.HeroId, out var placement))
            {
                participant.FinalPlacement = placement;
                await unitOfWork.TournamentParticipants.AddOrUpdateAsync(participant);
            }
        }

        tournament.Status = TournamentStatus.Completed;
        tournament.CompletedAt = completedAt;
        await unitOfWork.Tournaments.AddOrUpdateAsync(tournament);

        var pointsByHero = schedule.Awards
            .GroupBy(a => a.HeroId)
            .ToDictionary(g => g.Key, g => g.Sum(a => a.Points));
        await RatingLedger.ApplyAsync(unitOfWork, pointsByHero);

        await titleAwarder.AwardAsync(tournament, matches);

        await unitOfWork.SaveChangesAsync();

        return mapper.Map<Tournament>(tournament);
    }

    public async Task<IEnumerable<TournamentAward>> GetAwardsAsync(Guid tournamentId)
    {
        var awards = await unitOfWork.TournamentAwards.GetByTournamentAsync(tournamentId);
        return mapper.Map<IEnumerable<TournamentAward>>(awards);
    }

    public async Task CreateNextStagePlannedMatchesAsync(Guid tournamentId)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdWithParticipantsAsync(tournamentId);
        if (tournament == null)
        {
            throw new KeyNotFoundException($"No tournament with key {tournamentId}");
        }

        var generator = generatorFactory.TryCreate(tournament.Format)
            ?? throw new InvalidOperationException($"{tournament.Format} tournaments don't support automatic match generation.");

        var existingMatches = (await unitOfWork.Matches.GetByTournamentAsync(tournament.Id)).ToList();

        if (existingMatches.Any(m => m.IsPlanned))
        {
            throw new InvalidOperationException("Cannot generate matches for the next stage. Current stage is still in progress. Please finish current stage first.");
        }

        if (!generator.CanGenerateNext(tournament, existingMatches))
        {
            throw new InvalidOperationException("This tournament has no further matches to generate.");
        }

        var (pairings, stage, round) = generator.GenerateNext(tournament, tournament.Participants.ToList(), existingMatches);
        if (pairings.Count == 0)
        {
            throw new InvalidOperationException("Nobody is left to pair up for the next round.");
        }

        var heroesById = await LoadHeroesAsync(pairings);
        var maps = (await catalogMapCache.GetAsync()).ToList();

        var generatedMatches = new List<Match>();
        foreach (var pairing in pairings)
        {
            generatedMatches.Add(await BuildPlannedMatchAsync(tournament.Id, pairing, heroesById, maps, stage, round));
        }

        await PersistGeneratedMatchesAsync(tournament.Id, generatedMatches, stage);
    }

    private async Task<Dictionary<Guid, FighterHero>> LoadHeroesAsync(IReadOnlyList<TournamentPairing> pairings)
    {
        var heroIds = pairings.SelectMany(p => new[] { p.HeroId, p.OpponentHeroId }).Distinct();
        var heroesById = new Dictionary<Guid, FighterHero>();
        foreach (var heroId in heroIds)
        {
            var catalogHero = await catalogHeroCache.GetAsync(heroId);
            heroesById[heroId] = mapper.Map<FighterHero>(catalogHero);
        }

        return heroesById;
    }

    private async Task<Match> BuildPlannedMatchAsync(
        Guid tournamentId,
        TournamentPairing pairing,
        IReadOnlyDictionary<Guid, FighterHero> heroesById,
        List<CatalogMapDto> maps,
        Stage? stage,
        int? round)
    {
        var players = new List<PlayerDto?>
            {
                await playerCache.GetAsync(AndriiAndOlexPlayerIds.Andrii), // TODO: refactor
                await playerCache.GetAsync(AndriiAndOlexPlayerIds.Olex),
            }.Select(mapper.Map<FighterPlayer>)
            .ToList();
        var turns = new List<int> { 1, 2 };

        var fighter = new Fighter
        {
            Hero = heroesById[pairing.HeroId],
            HeroId = pairing.HeroId,
            Player = players.GetAndRemoveRandomItem(),
            Turn = turns.GetAndRemoveRandomItem()
        };
        var opponent = new Fighter
        {
            Hero = heroesById[pairing.OpponentHeroId],
            HeroId = pairing.OpponentHeroId,
            Player = players.GetAndRemoveRandomItem(),
            Turn = turns.GetAndRemoveRandomItem()
        };

        return new Match
        {
            Id = Guid.Empty,
            Stage = stage,
            Round = round,
            Fighters = new List<Fighter> { fighter, opponent },
            TournamentId = tournamentId,
            // Not GetAndRemoveRandomItem: maps is shared across every match this generation call
            // produces (including the 3 identical BO3 grand-final pairings), so depleting it would
            // crash once there are more matches in the batch than catalog maps.
            Map = maps.GetRandomItem(),
            IsPlanned = true
        };
    }

    private async Task PersistGeneratedMatchesAsync(Guid tournamentId, List<Match> matches, Stage? stage)
    {
        var matchEntities = mapper.Map<IEnumerable<MatchEntity>>(matches).ToList();

        foreach (var matchEntity in matchEntities)
        {
            await unitOfWork.Matches.AddAsync(matchEntity);
        }

        var tournament = await unitOfWork.Tournaments.GetByIdAsync(tournamentId);
        if (stage.HasValue)
        {
            tournament.CurrentStage = stage.Value;
        }

        tournament.Status = TournamentStatus.InProgress;
        await unitOfWork.Tournaments.AddOrUpdateAsync(tournament);

        await unitOfWork.SaveChangesAsync();
    }
}
