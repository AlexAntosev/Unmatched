namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Catalog;
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
    
    public async Task<SaveMatchResultDto> AddAsync(Match match)
    {
        var handler = _matchHandlerFactory.Create(match);
        await handler.HandleAsync(match);
        await _streakTitleHandler.HandleAsync();
        var rusherTitleEarned = await _rusherTitleHandler.HandleAsync(match);
        var punisherTitleEarned = await _punisherTitleHandler.HandleAsync(match);

        var titlesEarned = new List<TitleDto>();
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
        var result = new SaveMatchResultDto
            {
                WinnerHeroName = winnerHero.Name,
                WinnerMatchPoints = match.Fighters.First(f => f.IsWinner).MatchPoints.Value,
                LooserHeroName = looserHero.Name,
                LooserMatchPoints = match.Fighters.First(f => !f.IsWinner).MatchPoints.Value,
                TitlesEarned = titlesEarned
            };

        return result;
    }

    public Task<SaveMatchResultDto> AddAsync(MatchDto matchDto)
    {
        var match = _mapper.Map<Match>(matchDto);

        return AddAsync(match);
    }

    public Task<SaveMatchResultDto> UpdateAsync(MatchDto matchDto)
    {
        var match = _mapper.Map<Match>(matchDto);

        return AddAsync(match);
    }

    public async Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        var allMatches = await _unitOfWork.Matches.GetFinishedAsync();

        var matchLogs = new List<MatchLogDto>();
        foreach (var match in allMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = (await _unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId))
                .Select(fighter => _mapper.Map<FighterDto>(fighter))
                .ToArray();
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id)
    {
        var entities = await _unitOfWork.Matches.GetByTournamentAsync(id);

        var matches = _mapper.Map<IEnumerable<MatchDto>>(entities);
        foreach (var match in matches)
        {
            match.Fighters = match.Fighters.OrderBy(f => f.Turn);
        }
        
        return matches;
    }

    public async Task<MatchDto> GetAsync(Guid id)
    {
        var entity = await _unitOfWork.Matches.GetByIdAsync(id);
        var match = _mapper.Map<MatchDto>(entity);
        return match;
    }

    public async Task UpdateEpicAsync(Guid matchId, int epic)
    {
        var match = _unitOfWork.Matches.Query().FirstOrDefault(m => m.Id == matchId);
        if (match is not null)
        {
            match.Epic = epic;
            _unitOfWork.Matches.AddOrUpdate(match);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
