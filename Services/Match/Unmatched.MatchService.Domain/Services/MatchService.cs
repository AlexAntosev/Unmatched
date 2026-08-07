namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Contracts.Kafka;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Player;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.Domain.Validation;

public class MatchService(
    IMatchHandler matchHandler,
    RankedMatchDataValidator rankedMatchDataValidator,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    TitleEvaluator titleEvaluator,
    ICatalogHeroCache catalogHeroCache,
    IPlayerCache playerCache,
    IKafkaProducer kafkaProducer) : IMatchService
{
    public async Task<SaveMatchResult> AddOrUpdateAsync(Match matchDto)
    {
        var match = mapper.Map<MatchEntity>(matchDto);
        await rankedMatchDataValidator.ValidateAsync(match);

        // Captured before the handler applies this match's rating change, so the result can report
        // each fighter's before -> after movement. An Unranked match never changes ratings, so there
        // is no "before" worth fetching - both dictionaries stay empty and every fighter's rating
        // fields come back null, telling the UI to hide the rating pill rather than show a fake +0.
        var ratingsBefore = new Dictionary<Guid, int?>();
        if (match.IsRanked)
        {
            foreach (var heroId in match.Fighters.Select(f => f.HeroId).Distinct())
            {
                ratingsBefore[heroId] = (await unitOfWork.Ratings.GetByHeroIdAsync(heroId))?.Points;
            }
        }

        await matchHandler.HandleAsync(match);

        await FlagRecalculationIfAddedOutOfChronologicalOrderAsync(match);

        var addedEntity = await unitOfWork.Matches.GetByIdAsync(match.Id);

        var matchCreatedEvent = mapper.Map<MatchCreated>(addedEntity);
        var ratingsAfter = new Dictionary<Guid, int?>();
        foreach (var fighter in matchCreatedEvent.Fighters)
        {
            fighter.ResultRating = (await unitOfWork.Ratings.GetByHeroIdAsync(fighter.HeroId))?.Points;
            if (match.IsRanked)
            {
                ratingsAfter[fighter.HeroId] = fighter.ResultRating;
            }
        }
        await kafkaProducer.PublishAsync("match-created", matchCreatedEvent);

        var earnedTitles = new List<EarnedTitle>();
        if (match.GameMode != Enums.GameMode.Cooperative)
        {
            // TODO: move title logic to title microservice
            earnedTitles.AddRange(await titleEvaluator.EvaluateAsync(match));
        }
        var earnedTitlesByHero = earnedTitles.ToLookup(t => t.HeroId);

        var heroes = await catalogHeroCache.GetAsync();
        var players = await playerCache.GetAsync();
        var fighterResults = match.Fighters.Select(f => new FighterResult
            {
                HeroId = f.HeroId,
                HeroName = heroes.First(h => h.Id == f.HeroId).Name,
                PlayerName = players.First(p => p.Id == f.PlayerId).Name,
                MatchPoints = f.MatchPoints ?? 0,
                IsWinner = f.IsWinner,
                Placement = f.Placement,
                Team = f.Team,
                RatingBefore = ratingsBefore.GetValueOrDefault(f.HeroId),
                RatingAfter = ratingsAfter.GetValueOrDefault(f.HeroId),
                EarnedTitles = earnedTitlesByHero[f.HeroId].ToList()
            }).ToList();

        var result = new SaveMatchResult
            {
                GameMode = match.GameMode,
                FighterResults = fighterResults,
                PlayersWon = match.GameMode == Enums.GameMode.Cooperative ? match.Fighters.First().IsWinner : null,
                VillainName = matchCreatedEvent.Villain?.Name
            };

        return result;
    }

    public async Task<IEnumerable<Match>> GetAllAsync()
    {
        var entities = await unitOfWork.Matches.GetAsync();
        var matches = mapper.Map<IEnumerable<Match>>(entities);
        return matches;
    }

    public async Task<IEnumerable<Fighter>> GetAllFightersAsync()
    {
        var fighterEntities = await unitOfWork.Fighters.GetAsync();
        return fighterEntities.Select(mapper.Map<Fighter>);
    }

    public async Task<Match> GetAsync(Guid id)
    {
        var entity = await unitOfWork.Matches.GetByIdAsync(id);
        var match = mapper.Map<Match>(entity);
        return match;
    }

    public async Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid id)
    {
        var entities = await unitOfWork.Matches.GetByTournamentAsync(id);

        var matches = mapper.Map<IEnumerable<Match>>(entities);
        foreach (var match in matches)
        {
            match.Fighters = match.Fighters.OrderBy(f => f.Turn);
        }

        return matches;
    }

    public async Task<IEnumerable<Fighter>> GetFightersByHeroAsync(Guid heroId)
    {
        var fighterEntities = await unitOfWork.Fighters.GetFromFinishedMatchesByHeroIdAsync(heroId);
        return fighterEntities.Select(mapper.Map<Fighter>);
    }

    public async Task<IEnumerable<MatchLog>> GetFinishedByHeroAsync(Guid heroId)
    {
        var heroMatches = await unitOfWork.Matches.GetFinishedByHeroIdAsync(heroId);

        var matchLogs = new List<MatchLog>();

        foreach (var match in heroMatches)
        {
            var matchLog = mapper.Map<MatchLog>(match);

            var fighters = await unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId);

            matchLog.Fighters = mapper.Map<List<Fighter>>(fighters);

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchLog>> GetFinishedByMapAsync(Guid mapId)
    {
        var mapMatches = await unitOfWork.Matches.GetFinishedByMapIdAsync(mapId);

        var matchLogs = new List<MatchLog>();

        foreach (var match in mapMatches)
        {
            var matchLog = mapper.Map<MatchLog>(match);

            var fighters = await unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId);

            matchLog.Fighters = mapper.Map<List<Fighter>>(fighters);

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchLog>> GetFinishedByPlayerAsync(Guid playerId)
    {
        var playerMatches = await unitOfWork.Matches.GetFinishedByPlayerIdAsync(playerId);

        var matchLogs = new List<MatchLog>();

        foreach (var match in playerMatches)
        {
            var matchLog = mapper.Map<MatchLog>(match);

            var fighters = await unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId);

            matchLog.Fighters = mapper.Map<List<Fighter>>(fighters);

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchLog>> GetFinishedByVillainAsync(Guid villainId)
    {
        var villainMatches = await unitOfWork.Matches.GetFinishedByVillainIdAsync(villainId);

        var matchLogs = new List<MatchLog>();

        foreach (var match in villainMatches)
        {
            var matchLog = mapper.Map<MatchLog>(match);

            var fighters = await unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId);

            matchLog.Fighters = mapper.Map<List<Fighter>>(fighters);

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchLog>> GetFinishedByMinionAsync(Guid minionId)
    {
        var minionMatches = await unitOfWork.Matches.GetFinishedByMinionIdAsync(minionId);

        var matchLogs = new List<MatchLog>();

        foreach (var match in minionMatches)
        {
            var matchLog = mapper.Map<MatchLog>(match);

            var fighters = await unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId);

            matchLog.Fighters = mapper.Map<List<Fighter>>(fighters);

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchLog>> GetMatchLogAsync()
    {
        var allMatches = await unitOfWork.Matches.GetFinishedAsync();

        var matchLogs = new List<MatchLog>();
        foreach (var match in allMatches)
        {
            var matchLog = mapper.Map<MatchLog>(match);

            var fighters = (await unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId)).Select(mapper.Map<Fighter>).ToArray();
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    private async Task FlagRecalculationIfAddedOutOfChronologicalOrderAsync(MatchEntity match)
    {
        var latestOtherMatchDate = (await unitOfWork.Matches.GetAsync())
            .Where(m => m.Id != match.Id)
            .Select(m => (DateTime?)m.Date)
            .Max();

        var latestAwardDate = (await unitOfWork.TournamentAwards.GetAsync())
            .Select(a => (DateTime?)a.AwardedAt)
            .Max();

        var latestOtherOccurredAt = new[] { latestOtherMatchDate, latestAwardDate }.Max();

        if (latestOtherOccurredAt is not null && match.Date < latestOtherOccurredAt)
        {
            await unitOfWork.RatingRecalculationState.SetRecalculationRequiredAsync(true);
        }
    }

    public async Task UpdateEpicAsync(Guid matchId, int epic)
    {
        var match = await unitOfWork.Matches.GetByIdAsync(matchId);
        if (match is not null)
        {
            match.Epic = epic;
            await unitOfWork.Matches.AddOrUpdateAsync(match);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
