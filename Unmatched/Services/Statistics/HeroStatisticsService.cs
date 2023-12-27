namespace Unmatched.Services.Statistics;

using System.Globalization;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Repositories;

public class HeroStatisticsService : IHeroStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public HeroStatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<HeroStatisticsDto>> GetHeroesStatisticsAsync()
    {
        var heroes = await _unitOfWork.Heroes.Query().ToListAsync();
        var ratings = await _unitOfWork.Ratings.Query().ToListAsync();
        var fighters = await _unitOfWork.Fighters.Query().Include(x => x.Match).ToListAsync();

        var statistics = new List<HeroStatisticsDto>();

        foreach (var hero in heroes)
        {
            var heroDto = _mapper.Map<HeroDto>(hero);
            var points = ratings.FirstOrDefault(x => x.HeroId.Equals(hero.Id))?.Points ?? 0;
            var fights = fighters.Where(x => x.HeroId.Equals(hero.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var heroStatistics = new HeroStatisticsDto 
                {
                    Hero = heroDto,
                    HeroId = hero.Id,
                    HeroName = hero.Name,
                    Points = points,
                    TotalMatches = fights.Length,
                    TotalWins = fights.Count(x => x.IsWinner),
                    TotalLooses = fights.Count(x => x.IsWinner == false),
                    LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0,
                    IsRanged = hero.IsRanged
                };

            statistics.Add(heroStatistics);
        }

        return statistics;
    }
    
    public async Task<HeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId)
    {
        var hero = await _unitOfWork.Heroes.GetByIdAsync(heroId);
        var heroDto = _mapper.Map<HeroDto>(hero);
        var fights = await _unitOfWork.Fighters
            .Query()
            .Include(x => x.Match)
            .Where(x => x.HeroId.Equals(hero.Id))
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
        var rating = await _unitOfWork.Ratings.GetByHeroIdAsync(hero.Id);
        var points = rating?.Points ?? 0;
        
        var statistics = new HeroStatisticsDto
            {
                Hero = heroDto,
                HeroId = hero.Id,
                HeroName = hero.Name,
                Points = points,
                TotalMatches = fights.Count,
                TotalWins = fights.Count(x => x.IsWinner),
                TotalLooses = fights.Count(x => x.IsWinner == false),
                LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0,
                IsRanged = hero.IsRanged
            };
        
        return statistics;
    }
    
    public async Task<IEnumerable<MatchLogDto>> GetHeroMatchesAsync(Guid heroId)
    {
        var heroMatches = await _unitOfWork.Matches
            .Query()
            .Where(m => m.Fighters.Any(f => f.HeroId == heroId))
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .ToListAsync();

        var matchLogs = new List<MatchLogDto>();
        
        foreach (var match in heroMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = await _unitOfWork.Fighters
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

    public async Task<List<RatingChangeDto>> GetRatingChangesAsync(Guid heroId)
    {
        var ratingChanges = new List<RatingChangeDto>();
        var currentRating = await _unitOfWork.Ratings.GetByHeroIdAsync(heroId);
        ratingChanges.Add(new RatingChangeDto
            {
                Date = "Current",
                Rating = currentRating?.Points ?? 0
            });

        var points = currentRating?.Points ?? 0;

        var heroMatches = await _unitOfWork.Matches
            .Query()
            .Where(m => m.Fighters.Any(f => f.HeroId == heroId))
            .OrderByDescending(m => m.Date)
            .ToListAsync();
        
        foreach (var heroMatch in heroMatches)
        {
            var matchPoints = heroMatch.Fighters.FirstOrDefault(f => f.HeroId == heroId)?.MatchPoints ?? 0;
            points -= matchPoints;
            ratingChanges.Add(new RatingChangeDto
                {
                    Date = heroMatch.Date.ToShortDateString(),
                    Rating = points
                });
        }

        ratingChanges.Reverse();
        
        return ratingChanges;
    }
}
