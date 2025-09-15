namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.TitleHandlers;

public class MatchService(
    IMatchHandlerFactory matchHandlerFactory,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IStreakTitleHandler streakTitleHandler,
    IRusherTitleHandler rusherTitleHandler,
    IPunisherTitleHandler punisherTitleHandler,
    ICatalogHeroCache catalogHeroCache) : IMatchService
{
    public async Task<SaveMatchResult> AddOrUpdateAsync(Match matchDto)
    {
        var match = mapper.Map<MatchEntity>(matchDto);
        var handler = matchHandlerFactory.Create(match);
        await handler.HandleAsync(match);
        await streakTitleHandler.HandleAsync();
        var rusherTitleEarned = await rusherTitleHandler.HandleAsync(match);
        var punisherTitleEarned = await punisherTitleHandler.HandleAsync(match);

        var titlesEarned = new List<Title>();
        if (rusherTitleEarned is not null)
        {
            titlesEarned.Add(rusherTitleEarned);
        }

        if (punisherTitleEarned is not null)
        {
            titlesEarned.Add(punisherTitleEarned);
        }

        var heroes = await catalogHeroCache.GetAsync();
        var winnerHero = heroes.First(h => h.Id == match.Fighters.First(f => f.IsWinner).HeroId);
        var looserHero = heroes.First(h => h.Id == match.Fighters.First(f => !f.IsWinner).HeroId);
        var result = new SaveMatchResult
            {
                WinnerHeroName = winnerHero.Name,
                WinnerMatchPoints = match.Fighters.First(f => f.IsWinner).MatchPoints.Value,
                LooserHeroName = looserHero.Name,
                LooserMatchPoints = match.Fighters.First(f => !f.IsWinner).MatchPoints.Value,
                TitlesEarned = titlesEarned.Select(x => x.Name).ToList()
            };

        return result;
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

    public async Task<IEnumerable<Fighter>> GetAllFightersAsync()
    {
        var fighterEntities = await unitOfWork.Fighters.GetAsync();
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
