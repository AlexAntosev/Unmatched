using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Constants;
using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Repositories;

namespace Unmatched.Services;

public class UnmatchedService : IUnmatchedService
{
    private readonly IMapper _mapper;
    private readonly IMatchRepository _matchRepository;
    private readonly IFighterRepository _fighterRepository;
    private readonly IRatingCalculator _ratingCalculator;
    private readonly IPlayerRepository _playerRepository;
    private readonly IHeroRepository _heroRepository;
    private readonly IMapRepository _mapRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IRatingRepository _ratingRepository;

    public UnmatchedService(
        IMapper mapper,
        IMatchRepository matchRepository,
        IFighterRepository fighterRepository,
        IRatingCalculator ratingCalculator,
        IPlayerRepository playerRepository,
        IHeroRepository heroRepository,
        IMapRepository mapRepository,
        ITournamentRepository tournamentRepository,
        IRatingRepository ratingRepository)
    {
        _mapper = mapper;
        _matchRepository = matchRepository;
        _fighterRepository = fighterRepository;
        _ratingCalculator = ratingCalculator;
        _playerRepository = playerRepository;
        _heroRepository = heroRepository;
        _mapRepository = mapRepository;
        _tournamentRepository = tournamentRepository;
        _ratingRepository = ratingRepository;
    }
    
    public async Task AddDuelMatchAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto)
    {
        var match = _mapper.Map<Match>(matchDto);
        var fighter = _mapper.Map<Fighter>(matchDto);
        var opponent = _mapper.Map<Fighter>(matchDto);

        var createdMatch = await _matchRepository.AddAsync(match);

        fighter.MatchId = createdMatch.Id;
        opponent.MatchId = createdMatch.Id;
        
        var heroMatchPoints = await _ratingCalculator.CalculateAsync(fighter, opponent);
        
        var fighterRating = await _ratingRepository.GetByHeroIdAsync(fighter.HeroId) ?? new Rating {HeroId = fighter.HeroId};
        var opponentRating = await _ratingRepository.GetByHeroIdAsync(opponent.HeroId) ?? new Rating {HeroId = fighter.HeroId};

        var fighterMatchPoints = heroMatchPoints.First(x => x.HeroId == fighter.HeroId).Points;
        var opponentMatchPoints  = heroMatchPoints.First(x => x.HeroId == opponent.HeroId).Points;

        fighterRating.Points += fighterMatchPoints;
        opponentRating.Points += opponentMatchPoints;
        
        fighter.MatchPoints = fighterMatchPoints;
        opponent.MatchPoints = opponentMatchPoints;
        
        await _fighterRepository.AddAsync(fighter);
        await _fighterRepository.AddAsync(opponent);
        
        _ratingRepository.AddOrUpdate(fighterRating);
        _ratingRepository.AddOrUpdate(opponentRating);
        
        await _matchRepository.SaveChangesAsync();
        await _fighterRepository.SaveChangesAsync();
        await _ratingRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        var allMatches = await _matchRepository.Query().ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        foreach (var match in allMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);
            
            var fighters = (await _fighterRepository.Query().Where(x => matchLog.MatchId == x.MatchId).ToListAsync()).Select(fighter => _mapper.Map<FighterDto>(fighter)).ToArray();
            matchLog.Fighters = fighters;
            
            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
    {
        var entities = await _playerRepository.Query().ToListAsync();
        var players = _mapper.Map<IEnumerable<PlayerDto>>(entities);
        return players;
    }

    public async Task<IEnumerable<HeroDto>> GetAllHeroesAsync()
    {
        var entities = await _heroRepository.Query().ToListAsync();
        var heroes = _mapper.Map<IEnumerable<HeroDto>>(entities);
        return heroes;
    }

    public async Task<IEnumerable<MapDto>> GetAllMapsAsync()
    {
        var entities = await _mapRepository.Query().ToListAsync();
        var maps = _mapper.Map<IEnumerable<MapDto>>(entities);
        return maps;
    }

    public async Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync()
    {
        var entities = await _tournamentRepository.Query().ToListAsync();
        var tournaments = _mapper.Map<IEnumerable<TournamentDto>>(entities);
        return tournaments;
    }
}