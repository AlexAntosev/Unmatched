namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.TitleHandlers;

public class MatchService : IMatchService
{
    private readonly IMatchHandlerFactory _matchHandlerFactory;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IStreakTitleHandler _streakTitleHandler;
    private readonly IRusherTitleHandler _rusherTitleHandler;
    private readonly IPunisherTitleHandler _punisherTitleHandler;

    private readonly ICatalogHeroCache _catalogHeroCache;

    public MatchService(
        IMatchHandlerFactory matchHandlerFactory,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IStreakTitleHandler streakTitleHandler,
        IRusherTitleHandler rusherTitleHandler,
        IPunisherTitleHandler punisherTitleHandler,
        ICatalogHeroCache catalogHeroCache)
    {
        _matchHandlerFactory = matchHandlerFactory;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _streakTitleHandler = streakTitleHandler;
        _rusherTitleHandler = rusherTitleHandler;
        _punisherTitleHandler = punisherTitleHandler;
        _catalogHeroCache = catalogHeroCache;
    }

    public async Task<SaveMatchResult> AddOrUpdateAsync(Match matchDto)
    {
        var match = _mapper.Map<MatchEntity>(matchDto);
        var handler = _matchHandlerFactory.Create(match);
        await handler.HandleAsync(match);
        await _streakTitleHandler.HandleAsync();
        var rusherTitleEarned = await _rusherTitleHandler.HandleAsync(match);
        var punisherTitleEarned = await _punisherTitleHandler.HandleAsync(match);

        var titlesEarned = new List<Title>();
        if (rusherTitleEarned is not null)
        {
            titlesEarned.Add(rusherTitleEarned);
        }
        if (punisherTitleEarned is not null)
        {
            titlesEarned.Add(punisherTitleEarned);
        }

        var heroes = await _catalogHeroCache.GetAsync();
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

    public async Task<IEnumerable<MatchLog>> GetMatchLogAsync()
    {
        var allMatches = await _unitOfWork.Matches.GetFinishedAsync();

        var matchLogs = new List<MatchLog>();
        foreach (var match in allMatches)
        {
            var matchLog = _mapper.Map<MatchLog>(match);

            var fighters = (await _unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId))
                .Select(fighter => _mapper.Map<Fighter>(fighter))
                .ToArray();
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid id)
    {
        var entities = await _unitOfWork.Matches.GetByTournamentAsync(id);

        var matches = _mapper.Map<IEnumerable<Match>>(entities);
        foreach (var match in matches)
        {
            match.Fighters = match.Fighters.OrderBy(f => f.Turn);
        }
        
        return matches;
    }

    public async Task<Match> GetAsync(Guid id)
    {
        var entity = await _unitOfWork.Matches.GetByIdAsync(id);
        var match = _mapper.Map<Match>(entity);
        return match;
    }

    public async Task UpdateEpicAsync(Guid matchId, int epic)
    {
        var match = await _unitOfWork.Matches.GetByIdAsync(matchId);
        if (match is not null)
        {
            match.Epic = epic;
            await _unitOfWork.Matches.AddOrUpdateAsync(match);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
