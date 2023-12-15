namespace Unmatched.Services;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Unmatched.Dtos;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class UnmatchedService : IUnmatchedService
{
    private readonly IFighterRepository _fighterRepository;

    private readonly IHeroRepository _heroRepository;

    private readonly IMapper _mapper;

    private readonly IMapRepository _mapRepository;

    private readonly IMatchRepository _matchRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly IRatingRepository _ratingRepository;

    private readonly ITournamentRepository _tournamentRepository;

    public UnmatchedService(
        IMapper mapper,
        IMatchRepository matchRepository,
        IFighterRepository fighterRepository,
        IPlayerRepository playerRepository,
        IHeroRepository heroRepository,
        IMapRepository mapRepository,
        ITournamentRepository tournamentRepository,
        IRatingRepository ratingRepository)
    {
        _mapper = mapper;
        _matchRepository = matchRepository;
        _fighterRepository = fighterRepository;
        _playerRepository = playerRepository;
        _heroRepository = heroRepository;
        _mapRepository = mapRepository;
        _tournamentRepository = tournamentRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task<HeroDto> GetHeroByIdAsync(Guid heroId)
    {
        var entity = await _heroRepository.GetByIdAsync(heroId);
        var hero = _mapper.Map<HeroDto>(entity);
        return hero;
    }

    public async Task<IEnumerable<HeroDto>> GetAllHeroesAsync()
    {
        var entities = await _heroRepository.Query().Include(e => e.Sidekicks).ToListAsync();
        var heroes = _mapper.Map<IEnumerable<HeroDto>>(entities);
        return heroes;
    }

    public async Task<IEnumerable<MapDto>> GetAllMapsAsync()
    {
        var entities = await _mapRepository.Query().ToListAsync();
        var maps = _mapper.Map<IEnumerable<MapDto>>(entities);
        return maps;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
    {
        var entities = await _playerRepository.Query().ToListAsync();
        var players = _mapper.Map<IEnumerable<PlayerDto>>(entities);
        return players;
    }

    public async Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync()
    {
        var entities = await _tournamentRepository.Query().ToListAsync();
        var tournaments = _mapper.Map<IEnumerable<TournamentDto>>(entities);
        return tournaments;
    }

    public async Task<IEnumerable<RankedRatingHeroStatisticsDto>> GetRankedRatingAsync()
    {
        var heroes = await _heroRepository.Query().ToListAsync();
        var ratings = await _ratingRepository.Query().ToListAsync();
        var fighters = await _fighterRepository.Query().Include(x => x.Match).ToListAsync();

        var ratingDtos = new List<RankedRatingHeroStatisticsDto>();

        foreach (var hero in heroes)
        {
            var points = ratings.FirstOrDefault(x => x.HeroId.Equals(hero.Id))?.Points ?? 0;
            var heroParticipations = fighters.Where(x => x.HeroId.Equals(hero.Id) && x.Match.TournamentId != null).OrderByDescending(x => x.Match.Date).ToArray();

            var ratingHero = new RankedRatingHeroStatisticsDto()
                {
                    HeroName = hero.Name,
                    Points = points,
                    TotalMatches = heroParticipations.Length,
                    TotalWins = heroParticipations.Count(x => x.IsWinner),
                    TotalLooses = heroParticipations.Count(x => x.IsWinner == false),
                    LastMatchPoints = heroParticipations.FirstOrDefault()?.MatchPoints ?? 0
                };

            ratingDtos.Add(ratingHero);
        }

        return ratingDtos;
    }

    public async Task<IEnumerable<HeroStatisticsDto>> GetHeroesStatisticsAsync()
    {
        var heroes = await _heroRepository.Query().ToListAsync();
        var fighters = await _fighterRepository.Query().Include(x => x.Match).ToListAsync();

        var ratingDtos = new List<RankedRatingHeroStatisticsDto>();

        foreach (var hero in heroes)
        {
            var heroParticipations = fighters.Where(x => x.HeroId.Equals(hero.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var ratingHero = new RankedRatingHeroStatisticsDto()
                {
                    HeroId = hero.Id,
                    HeroName = hero.Name,
                    TotalMatches = heroParticipations.Length,
                    TotalWins = heroParticipations.Count(x => x.IsWinner),
                    TotalLooses = heroParticipations.Count(x => x.IsWinner == false),
                    LastMatchPoints = heroParticipations.FirstOrDefault()?.MatchPoints ?? 0
                };

            ratingDtos.Add(ratingHero);
        }

        return ratingDtos;
    }

    public async Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync()
    {
        var players = await _playerRepository.Query().ToListAsync();
        var fighters = await _fighterRepository.Query().Include(x => x.Match).ToListAsync();

        var ratingDtos = new List<PlayerStatisticsDto>();

        foreach (var player in players)
        {
            var playerFighters = fighters.Where(x => x.PlayerId.Equals(player.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var ratingPlayer = new PlayerStatisticsDto()
                {
                    PlayerId = player.Id,
                    PlayerName = player.Name,
                    TotalMatches = playerFighters.Length,
                    TotalWins = playerFighters.Count(x => x.IsWinner),
                    TotalLooses = playerFighters.Count(x => x.IsWinner == false),
                    LastMatchPoints = playerFighters.FirstOrDefault()?.MatchPoints ?? 0
                };

            ratingDtos.Add(ratingPlayer);
        }

        return ratingDtos;
    }

    public async Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync()
    {
        var maps = await _mapRepository.Query().ToListAsync();
        var mapMatches = await _matchRepository
            .Query()
            .ToListAsync();

        var ratingDtos = new List<MapStatisticsDto>();

        foreach (var map in maps)
        {
            var mapFighters = mapMatches.Where(x => x.MapId.Equals(map.Id)).OrderByDescending(x => x.Date).ToArray();

            var ratingPlayer = new MapStatisticsDto()
                {
                    MapId = map.Id,
                    MapName = map.Name,
                    TotalMatches = mapFighters.Length,
                };

            ratingDtos.Add(ratingPlayer);
        }

        return ratingDtos;
    }

    public async Task<HeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId)
    {
        var hero = await _heroRepository.GetByIdAsync(heroId);
        
        var heroFighters = await _fighterRepository
            .Query()
            .Include(x => x.Match)
            .Where(x => x.HeroId.Equals(hero.Id))
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
        
        var statistics = new HeroStatisticsDto()
            {
                HeroId = hero.Id,
                HeroName = hero.Name,
                TotalMatches = heroFighters.Count,
                TotalWins = heroFighters.Count(x => x.IsWinner),
                TotalLooses = heroFighters.Count(x => x.IsWinner == false),
                LastMatchPoints = heroFighters.FirstOrDefault()?.MatchPoints ?? 0
            };
        
        return statistics;
    }

    public async Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        
        var playerFighters = await _fighterRepository
            .Query()
            .Include(x => x.Match)
            .Where(x => x.PlayerId.Equals(player.Id))
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
        
        var statistics = new PlayerStatisticsDto()
            {
                PlayerId = player.Id,
                PlayerName = player.Name,
                TotalMatches = playerFighters.Count,
                TotalWins = playerFighters.Count(x => x.IsWinner),
                TotalLooses = playerFighters.Count(x => x.IsWinner == false),
                LastMatchPoints = playerFighters.FirstOrDefault()?.MatchPoints ?? 0
            };
        
        return statistics;
    }

    public async Task<MapStatisticsDto> GetMapStatisticsAsync(Guid mapId)
    {
        var map = await _mapRepository.GetByIdAsync(mapId);
        
        var mapMatches = await _matchRepository
            .Query()
            .Where(x => x.MapId.Equals(map.Id))
            .OrderByDescending(x => x.Date)
            .ToListAsync();
        
        var statistics = new MapStatisticsDto()
            {
                MapId = map.Id,
                MapName = map.Name,
                TotalMatches = mapMatches.Count,
            };
        
        return statistics;
    }

    public async Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        var allMatches = await _matchRepository.Query().Include(x => x.Map).Include(x => x.Tournament).ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        foreach (var match in allMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = (await _fighterRepository.Query().Include(x => x.Player).Include(x => x.Hero).Where(x => matchLog.MatchId == x.MatchId).ToListAsync())
                .Select(fighter => _mapper.Map<FighterDto>(fighter))
                .ToArray();
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchLogDto>> GetHeroMatchesAsync(Guid heroId)
    {
        var heroMatches = await _matchRepository
            .Query()
            .Where(m => m.Fighters.Any(f => f.HeroId == heroId))
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        foreach (var match in heroMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = await _fighterRepository
                .Query()
                .Where(x => matchLog.MatchId == x.MatchId)
                .Include(x => x.Player)
                .Include(x => x.Hero)
                .Select(fighter => _mapper.Map<FighterDto>(fighter))
                .ToListAsync();
            
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchLogDto>> GetPlayerMatchesAsync(Guid playerId)
    {
        var playerMatches = await _matchRepository
            .Query()
            .Where(m => m.Fighters.Any(f => f.PlayerId == playerId))
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        foreach (var match in playerMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = await _fighterRepository
                .Query()
                .Where(x => matchLog.MatchId == x.MatchId)
                .Include(x => x.Player)
                .Include(x => x.Hero)
                .Select(fighter => _mapper.Map<FighterDto>(fighter))
                .ToListAsync();
            
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task<IEnumerable<MatchLogDto>> GetMapMatchesAsync(Guid mapId)
    {
        var mapMatches = await _matchRepository
            .Query()
            .Where(m => m.MapId == mapId)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        foreach (var match in mapMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = await _fighterRepository
                .Query()
                .Where(x => matchLog.MatchId == x.MatchId)
                .Include(x => x.Player)
                .Include(x => x.Hero)
                .Select(fighter => _mapper.Map<FighterDto>(fighter))
                .ToListAsync();
            
            matchLog.Fighters = fighters;

            matchLogs.Add(matchLog);
        }

        return matchLogs;
    }

    public async Task RecalculateRating()
    {
        await _ratingRepository.ClearAllRatingsAsync();
        var rankedMatches = await _matchRepository.Query().Where(x => x.Tournament != null).OrderBy(x => x.Date).ToListAsync();

        foreach (var match in rankedMatches)
        {
            
        }
    }
}
