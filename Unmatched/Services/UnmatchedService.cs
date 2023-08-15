namespace Unmatched.Services;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Repositories;

public class UnmatchedService : IUnmatchedService
{
    private readonly IFighterRepository _fighterRepository;

    private readonly IHeroRepository _heroRepository;

    private readonly IMapper _mapper;

    private readonly IMapRepository _mapRepository;

    private readonly IMatchRepository _matchRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly IRatingCalculator _ratingCalculator;

    private readonly IRatingRepository _ratingRepository;

    private readonly ITournamentRepository _tournamentRepository;

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
        var fighter = _mapper.Map<Fighter>(fighterDto);
        var opponent = _mapper.Map<Fighter>(opponentDto);

        var createdMatch = await _matchRepository.AddAsync(match);

        fighter.MatchId = createdMatch.Id;
        opponent.MatchId = createdMatch.Id;

        var heroMatchPoints = await _ratingCalculator.CalculateAsync(fighter, opponent);

        var fighterRating = await _ratingRepository.GetByHeroIdAsync(fighter.HeroId)
         ?? new Rating
                {
                    HeroId = fighter.HeroId
                };
        var opponentRating = await _ratingRepository.GetByHeroIdAsync(opponent.HeroId)
         ?? new Rating
                {
                    HeroId = fighter.HeroId
                };

        var fighterMatchPoints = heroMatchPoints.First(x => x.HeroId == fighter.HeroId).Points;
        var opponentMatchPoints = heroMatchPoints.First(x => x.HeroId == opponent.HeroId).Points;

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

    public async Task<IEnumerable<HeroStatisticsDto>> GetStatisticsAsync()
    {
        var heroes = await _heroRepository.Query().ToListAsync();
        var fighters = await _fighterRepository.Query().Include(x => x.Match).ToListAsync();

        var ratingDtos = new List<RankedRatingHeroStatisticsDto>();

        foreach (var hero in heroes)
        {
            var heroParticipations = fighters.Where(x => x.HeroId.Equals(hero.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var ratingHero = new RankedRatingHeroStatisticsDto()
                {
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
}
