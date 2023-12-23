namespace Unmatched.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.MatchHandlers;

public class MatchService : IMatchService
{
    private readonly IMatchHandlerFactory _matchHandlerFactory;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public MatchService(IMatchHandlerFactory matchHandlerFactory, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _matchHandlerFactory = matchHandlerFactory;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    
    public Task AddAsync(Match match)
    {
        var handler = _matchHandlerFactory.Create(match);
        return handler.HandleAsync(match);
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
    
    public async Task<IEnumerable<HeroDto>> GetAllHeroesAsync()
    {
        var entities = await _unitOfWork.Heroes.Query().Include(e => e.Sidekicks).ToListAsync();
        var heroes = _mapper.Map<IEnumerable<HeroDto>>(entities);
        return heroes;
    }

    public async Task<IEnumerable<MapDto>> GetAllMapsAsync()
    {
        var entities = await _unitOfWork.Maps.Query().ToListAsync();
        var maps = _mapper.Map<IEnumerable<MapDto>>(entities);
        return maps;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
    {
        var entities = await _unitOfWork.Players.Query().ToListAsync();
        var players = _mapper.Map<IEnumerable<PlayerDto>>(entities);
        return players;
    }

    public async Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync()
    {
        var entities = await _unitOfWork.Tournaments.Query().ToListAsync();
        var tournaments = _mapper.Map<IEnumerable<TournamentDto>>(entities);
        return tournaments;
    }
    
    public async Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        var allMatches = await _unitOfWork.Matches.Query().Include(x => x.Map).Include(x => x.Tournament).ToListAsync();

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
}
