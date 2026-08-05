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
        tournament.Status = dto.Format == TournamentFormat.Bounty ? TournamentStatus.InProgress : TournamentStatus.Draft;
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

    public async Task<Tournament?> UpdateNameAsync(Guid id, string name)
    {
        // With participants, unlike UpdateImageAsync/UpdateTrophyImageAsync - the renamed tournament
        // is shown immediately from this response, and Tournament.razor's participant count would
        // otherwise flash to zero until the next full reload.
        var tournament = await unitOfWork.Tournaments.GetByIdWithParticipantsAsync(id);
        if (tournament is null)
        {
            return null;
        }

        tournament.Name = name;
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

        if (matches.Count == 0)
        {
            throw new InvalidOperationException("Cannot complete this tournament: it has no matches.");
        }

        // the last match's Date, not "now" - completion is often clicked well after the matches were
        // actually played/logged, and backdating the award to when the tournament really finished keeps
        // it in its correct chronological position for the rating replay (see RatingTimeline).
        var completedAt = matches.Max(m => m.Date);
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

    /// <summary>Regenerates a completed tournament's award points and placements from its current
    /// participants/matches - for correcting a completion that ran against a wrong participant set
    /// (e.g. one backfilled from every match ever tied to the tournament instead of just its bracket).
    /// Awards replay at the tournament's original <see cref="TournamentEntity.CompletedAt"/> so they keep
    /// their original position in the rating timeline; titles aren't re-awarded here.</summary>
    public async Task RecomputeCompletionAwardsAsync(Guid tournamentId)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdWithParticipantsAsync(tournamentId)
            ?? throw new KeyNotFoundException($"No tournament with key {tournamentId}");

        if (tournament.Status != TournamentStatus.Completed)
        {
            throw new InvalidOperationException("Only a completed tournament's awards can be recomputed.");
        }

        var matches = await unitOfWork.Matches.GetByTournamentAsync(tournamentId);
        var existingAwards = await unitOfWork.TournamentAwards.GetByTournamentAsync(tournamentId);
        foreach (var award in existingAwards)
        {
            await unitOfWork.TournamentAwards.Delete(award.Id);
        }

        var schedule = awardScheduler.Schedule(tournament, matches, tournament.CompletedAt!.Value);

        foreach (var award in schedule.Awards)
        {
            await unitOfWork.TournamentAwards.AddAsync(award);
        }

        foreach (var participant in tournament.Participants)
        {
            participant.FinalPlacement = schedule.FinalPlacements.GetValueOrDefault(participant.HeroId);
            await unitOfWork.TournamentParticipants.AddOrUpdateAsync(participant);
        }

        await unitOfWork.SaveChangesAsync();
    }

    /// <summary>Run at startup (after migrations): finds every completed tournament whose stored awards
    /// no longer match its current participants - e.g. a participant-correcting migration ran, or a
    /// database restore reverted an earlier <see cref="RecomputeCompletionAwardsAsync"/> - and recomputes
    /// just those, so a stale award set never lingers silently.</summary>
    public async Task ReconcileCompletionAwardsAsync()
    {
        var completedTournaments = await unitOfWork.Tournaments.GetCompletedWithParticipantsAsync();
        foreach (var tournament in completedTournaments)
        {
            var participantHeroIds = tournament.Participants.Select(p => p.HeroId).ToHashSet();
            var awardHeroIds = (await unitOfWork.TournamentAwards.GetByTournamentAsync(tournament.Id))
                .Select(a => a.HeroId)
                .ToHashSet();

            if (!participantHeroIds.SetEquals(awardHeroIds))
            {
                await RecomputeCompletionAwardsAsync(tournament.Id);
            }
        }
    }

    public async Task<IEnumerable<TournamentAward>> GetAwardsAsync(Guid tournamentId)
    {
        var awards = await unitOfWork.TournamentAwards.GetByTournamentAsync(tournamentId);
        return mapper.Map<IEnumerable<TournamentAward>>(awards);
    }

    public async Task<BountyState> GetBountyStateAsync(Guid tournamentId)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(tournamentId)
            ?? throw new KeyNotFoundException($"No tournament with key {tournamentId}");
        var matches = await unitOfWork.Matches.GetByTournamentAsync(tournamentId);
        var poolRow = (await unitOfWork.TournamentAwards.GetByTournamentAsync(tournamentId))
            .FirstOrDefault(a => a.AwardKind == TournamentAwardKind.BountyPool);

        var defenseCount = BountyChampionship.Compute(tournament.StartingChampionId, matches).DefenseCount;
        return new BountyState
        {
            ChampionHeroId = poolRow?.HeroId ?? tournament.StartingChampionId,
            DefenseCount = defenseCount,
            BankPoints = poolRow?.Points ?? 0
        };
    }

    public async Task<Match> CreateBountyChallengeAsync(
        Guid tournamentId, Guid challengerHeroId, Guid championPlayerId, Guid challengerPlayerId, Guid mapId)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(tournamentId)
            ?? throw new KeyNotFoundException($"No tournament with key {tournamentId}");
        if (tournament.Format != TournamentFormat.Bounty)
        {
            throw new InvalidOperationException("Only Bounty tournaments support challenges.");
        }

        var poolRow = (await unitOfWork.TournamentAwards.GetByTournamentAsync(tournamentId))
            .FirstOrDefault(a => a.AwardKind == TournamentAwardKind.BountyPool);
        var championId = poolRow?.HeroId ?? tournament.StartingChampionId
            ?? throw new InvalidOperationException("This Bounty pool has no starting champion set.");

        if (championId == challengerHeroId)
        {
            throw new InvalidOperationException("The challenger cannot be the current champion.");
        }

        var heroesById = new Dictionary<Guid, FighterHero>
        {
            [championId] = mapper.Map<FighterHero>(await catalogHeroCache.GetAsync(championId)),
            [challengerHeroId] = mapper.Map<FighterHero>(await catalogHeroCache.GetAsync(challengerHeroId))
        };
        var map = (await catalogMapCache.GetAsync()).First(m => m.Id == mapId);

        var champion = new Fighter
        {
            Hero = heroesById[championId],
            HeroId = championId,
            Player = mapper.Map<FighterPlayer>(await playerCache.GetAsync(championPlayerId)),
            Turn = 1
        };
        var challenger = new Fighter
        {
            Hero = heroesById[challengerHeroId],
            HeroId = challengerHeroId,
            Player = mapper.Map<FighterPlayer>(await playerCache.GetAsync(challengerPlayerId)),
            Turn = 2
        };
        SetDefaultStats(champion);
        SetDefaultStats(challenger);

        var match = new Match
        {
            Id = Guid.Empty,
            Date = DateTime.Now,
            Fighters = new List<Fighter> { champion, challenger },
            TournamentId = tournamentId,
            Map = map,
            IsPlanned = true,
            IsRanked = true
        };

        var matchEntity = mapper.Map<MatchEntity>(match);
        var added = await unitOfWork.Matches.AddAsync(matchEntity);
        await unitOfWork.SaveChangesAsync();

        // Returned as the already-populated model built above, not re-mapped from the persisted entity:
        // Fighter.Hero there comes from FighterHeroResolver, which round-trips through the catalog cache
        // for no reason when this method already built it by hand.
        match.Id = added.Id;
        return match;
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
        SetDefaultStats(fighter);
        SetDefaultStats(opponent);

        return new Match
        {
            Id = Guid.Empty,
            Date = DateTime.Now,
            Stage = stage,
            Round = round,
            Fighters = new List<Fighter> { fighter, opponent },
            TournamentId = tournamentId,
            // Not GetAndRemoveRandomItem: maps is shared across every match this generation call
            // produces (including the 3 identical BO3 grand-final pairings), so depleting it would
            // crash once there are more matches in the batch than catalog maps.
            Map = maps.GetRandomItem(),
            IsPlanned = true,
            IsRanked = true
        };
    }

    /// <summary>A freshly generated tournament match starts fully healthy - full HP, full deck, full
    /// sidekick HP - the same defaults a manually-added match's fighters get (see
    /// UiFighterDto.SetDefaultData on the client), so the match sheet opens ready to edit down from a
    /// real starting point instead of blank fields.</summary>
    private static void SetDefaultStats(Fighter fighter)
    {
        fighter.HpLeft = fighter.Hero!.Hp;
        fighter.CardsLeft = fighter.Hero.DeckSize;
        fighter.SidekickHpLeft = fighter.Hero.Sidekicks.Sum(s => s.Hp * s.Count);
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
