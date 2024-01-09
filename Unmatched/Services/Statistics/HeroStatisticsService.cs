namespace Unmatched.Services.Statistics;

using AutoMapper;
using Unmatched.Dtos;
using Unmatched.Entities;
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
        var heroes = await _unitOfWork.Heroes.GetAsync();
        var ratings = await _unitOfWork.Ratings.GetAsync();
        var fighters = await _unitOfWork.Fighters.GetFromFinishedMatchesAsync();

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
        var fights = await _unitOfWork.Fighters.GetFromFinishedMatchesByHeroIdAsync(heroId);
        var rating = await _unitOfWork.Ratings.GetByHeroIdAsync(hero.Id);
        var points = rating?.Points ?? 0;
        var titles = await _unitOfWork.Titles.GetByHeroId(heroId);
        var titlesDto = _mapper.Map<IEnumerable<TitleDto>>(titles);
        var place = await GetHeroPlace(rating);

        var statistics = new HeroStatisticsDto
            {
                Hero = heroDto,
                HeroId = hero.Id,
                HeroName = hero.Name,
                Points = points,
                Place = place,
                TotalMatches = fights.Count,
                TotalWins = fights.Count(x => x.IsWinner),
                TotalLooses = fights.Count(x => x.IsWinner == false),
                LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0,
                IsRanged = hero.IsRanged,
                Titles = titlesDto
            };
        
        return statistics;
    }

    public async Task<IEnumerable<MatchLogDto>> GetHeroMatchesAsync(Guid heroId)
    {
        var heroMatches = await _unitOfWork.Matches.GetFinishedByHeroIdAsync(heroId);

        var matchLogs = new List<MatchLogDto>();
        
        foreach (var match in heroMatches)
        {
            var matchLog = _mapper.Map<MatchLogDto>(match);

            var fighters = await _unitOfWork.Fighters.GetByMatchIdAsync(matchLog.MatchId);
            
            matchLog.Fighters = _mapper.Map<List<FighterDto>>(fighters);

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

        var heroMatches = await _unitOfWork.Matches.GetFinishedByHeroIdAsync(heroId);
        
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
    
    private async Task<int> GetHeroPlace(Rating? rating)
    {
        var place = 0;
        if (rating is null)
        {
            return place;
        }

        var ratings = await _unitOfWork.Ratings.GetAsync();

        if (ratings.Any(r => r.Id == rating.Id))
        {
            place = ratings.IndexOf(rating) + 1;
        }

        return place;
    }
}
