namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Dtos;
using Unmatched.Services.MatchHandlers;
using Unmatched.Services.TitleHandlers;

public class MatchService : IMatchService
{
    private readonly IMatchHandlerFactory _matchHandlerFactory;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IStreakTitleHandler _streakTitleHandler;
    private readonly IRusherTitleHandler _rusherTitleHandler;

    public MatchService(
        IMatchHandlerFactory matchHandlerFactory,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IStreakTitleHandler streakTitleHandler,
        IRusherTitleHandler rusherTitleHandler)
    {
        _matchHandlerFactory = matchHandlerFactory;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _streakTitleHandler = streakTitleHandler;
        _rusherTitleHandler = rusherTitleHandler;
    }
    
    public async Task AddAsync(Match match)
    {
        var handler = _matchHandlerFactory.Create(match);
        await handler.HandleAsync(match);
        await _streakTitleHandler.HandleAsync();
        await _rusherTitleHandler.HandleAsync(match);
    }

    public Task AddAsync(MatchDto matchDto)
    {
        var match = _mapper.Map<Match>(matchDto);

        return AddAsync(match);
    }

    public Task UpdateAsync(MatchDto matchDto)
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
}
