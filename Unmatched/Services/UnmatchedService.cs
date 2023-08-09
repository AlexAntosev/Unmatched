using AutoMapper;
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

    public UnmatchedService(
        IMapper mapper,
        IMatchRepository matchRepository,
        IFighterRepository fighterRepository,
        IRatingCalculator ratingCalculator,
        IPlayerRepository playerRepository,
        IHeroRepository heroRepository)
    {
        _mapper = mapper;
        _matchRepository = matchRepository;
        _fighterRepository = fighterRepository;
        _ratingCalculator = ratingCalculator;
        _playerRepository = playerRepository;
        _heroRepository = heroRepository;
    }
    
    public async Task AddDuelMatchAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto)
    {
        var match = _mapper.Map<Match>(matchDto);
        var fighter = _mapper.Map<Fighter>(matchDto);
        var opponent = _mapper.Map<Fighter>(matchDto);

        var createdMatch = await _matchRepository.AddAsync(match);

        fighter.MatchId = createdMatch.Id;
        opponent.MatchId = createdMatch.Id;

        await _fighterRepository.AddAsync(fighter);
        await _fighterRepository.AddAsync(opponent);

        if (match.Tournament is not null)
        {
            await _ratingCalculator.CalculateAsync(fighter, opponent, match.Tournament);
        }

        await _matchRepository.SaveChangesAsync();
        await _fighterRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        return await Task.FromResult(new List<MatchLogDto>());
    }

    public IEnumerable<PlayerDto> GetAllPlayers()
    {
        var entities = _playerRepository.Query();
        var players = _mapper.Map<IEnumerable<PlayerDto>>(entities);
        return players;
    }

    public IEnumerable<HeroDto> GetAllHeroes()
    {
        var entities = _heroRepository.Query();
        var heroes = _mapper.Map<IEnumerable<HeroDto>>(entities);
        return heroes;
    }
}