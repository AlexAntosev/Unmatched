namespace Unmatched.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Enums;
using Unmatched.Repositories;
using Unmatched.Services.MatchHandlers;
using Unmatched.Services.TitleHandlers;

public class MatchService : IMatchService
{
    private readonly IMatchHandlerFactory _matchHandlerFactory;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IStreakTitleHandler _streakTitleHandler;

    public MatchService(
        IMatchHandlerFactory matchHandlerFactory,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IStreakTitleHandler streakTitleHandler)
    {
        _matchHandlerFactory = matchHandlerFactory;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _streakTitleHandler = streakTitleHandler;
    }
    
    public async Task AddAsync(Match match)
    {
        var handler = _matchHandlerFactory.Create(match);
        await handler.HandleAsync(match);
        await _streakTitleHandler.HandleAsync();
    }

    public Task AddAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto)
    {
        var match = _mapper.Map<Match>(matchDto);
        var firstFighter = _mapper.Map<Fighter>(fighterDto);
        var secondFighter = _mapper.Map<Fighter>(opponentDto);

        match.Fighters = new List<Fighter>
            {
                firstFighter,
                secondFighter
            };

        return AddAsync(match);
    }

    public Task UpdateAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto)
    {
        var match = _mapper.Map<Match>(matchDto);
        var firstFighter = _mapper.Map<Fighter>(fighterDto);
        var secondFighter = _mapper.Map<Fighter>(opponentDto);

        match.Fighters = new List<Fighter>
            {
                firstFighter,
                secondFighter
            };

        return AddAsync(match);
    }

    public async Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        var allMatches = await _unitOfWork.Matches.Query().Include(x => x.Map).Include(x => x.Tournament).Where(m => !m.IsPlanned).ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        foreach (var match in allMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = (await _unitOfWork.Fighters
                    .Query()
                    .Include(x => x.Player)
                    .Include(x => x.Hero)
                    .Where(x => matchLog.MatchId == x.MatchId)
                    .ToListAsync())
                .Select(fighter => _mapper.Map<FighterDto>(fighter))
                .ToArray();
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id, Stage? stage = null)
    {
        var query = _unitOfWork.Matches.Query()
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .Include(m => m.Fighters)
                .ThenInclude(f => f.Hero)
            .Include(m => m.Fighters)
                .ThenInclude(f => f.Player)
            .Where(m => m.TournamentId == id && m.Stage == stage);

        var entities = await query.ToListAsync();

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
