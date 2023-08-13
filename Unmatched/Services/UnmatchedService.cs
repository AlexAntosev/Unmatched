using AutoMapper;
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
        return await Task.FromResult(new List<MatchLogDto>()
        {
            new MatchLogDto()
            {
                Date = DateTime.Today,
                Comment = "Comment",
                MapName = MapNames.RaptorPaddock,
                TournamentName = TournamentNames.GoldenHalatLeague,
                Fighters = new List<FighterDto>()
                {
                    new FighterDto()
                    {
                        Player = new PlayerDto()
                        {
                            Name = PlayerNames.Oleksandr
                        },
                        Hero = new HeroDto()
                        {
                            Name = HeroNames.Achilles,
                            Sidekicks = new List<SidekickDto>()
                            {
                                new SidekickDto()
                                {
                                    Name = SidekickNames.Patroclus
                                }
                            }
                        },
                        HpLeft = 10,
                        SidekickHpLeft = 0,
                        CardsLeft = 4,
                        ItemsUsed = 0,
                        IsWinner = true,
                        Turn = 2
                    },
                    new FighterDto()
                    {
                        Player = new PlayerDto()
                        {
                            Name = PlayerNames.Andrii
                        },
                        Hero = new HeroDto()
                        {
                            Name = HeroNames.SherlokHolmes,
                            Sidekicks = new List<SidekickDto>()
                            {
                                new SidekickDto()
                                {
                                    Name = SidekickNames.DrWatson
                                }
                            }
                        },
                        HpLeft = 0,
                        SidekickHpLeft = 2,
                        CardsLeft = 9,
                        ItemsUsed = 0,
                        IsWinner = false,
                        Turn = 1
                    }
                }
            }
        });
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

    public IEnumerable<MapDto> GetAllMaps()
    {
        var entities = _mapRepository.Query();
        var maps = _mapper.Map<IEnumerable<MapDto>>(entities);
        return maps;
    }

    public IEnumerable<TournamentDto> GetAllTournaments()
    {
        var entities = _tournamentRepository.Query();
        var tournaments = _mapper.Map<IEnumerable<TournamentDto>>(entities);
        return tournaments;
    }
}