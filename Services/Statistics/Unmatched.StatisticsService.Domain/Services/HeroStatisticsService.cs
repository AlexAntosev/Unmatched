namespace Unmatched.StatisticsService.Domain.Services;

using AutoMapper;

using Unmatched.StatisticsService.Domain.Models;

public class HeroStatisticsService : IHeroStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ICatalogHeroCache _catalogHeroCache;

    public HeroStatisticsService(IUnitOfWork unitOfWork, IMapper mapper, ICatalogHeroCache catalogHeroCache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _catalogHeroCache = catalogHeroCache;
    }
    
    public async Task<IEnumerable<HeroStats>> GetHeroesStatisticsAsync()
    {
        var heroes = await _catalogHeroCache.GetAsync();
        var ratings = await _unitOfWork.Ratings.GetAsync();
        var fighters = await _unitOfWork.Fighters.GetFromFinishedMatchesAsync();

        var statistics = new List<HeroStats>();

        foreach (var hero in heroes)
        {
            var heroDto = _mapper.Map<HeroDto>(hero);
            var points = ratings.FirstOrDefault(x => x.HeroId.Equals(hero.Id))?.Points ?? 0;
            var fights = fighters.Where(x => x.HeroId.Equals(hero.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var heroStatistics = new HeroStats 
                {
                    Hero = heroDto,
                    HeroId = hero.Id,
                    Points = points,
                    TotalMatches = fights.Length,
                    TotalWins = fights.Count(x => x.IsWinner),
                    TotalLooses = fights.Count(x => x.IsWinner == false),
                    LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0
                };

            statistics.Add(heroStatistics);
        }

        return statistics;
    }
    
    public async Task<HeroStats> GetHeroStatisticsAsync(Guid heroId)
    {
        var hero = await _catalogHeroCache.GetAsync(heroId);
        var heroDto = _mapper.Map<HeroDto>(hero);
        var fights = await _unitOfWork.Fighters.GetFromFinishedMatchesByHeroIdAsync(heroId);
        var rating = await _unitOfWork.Ratings.GetByHeroIdAsync(hero.Id);
        var points = rating?.Points ?? 0;
        var titles = await _unitOfWork.Titles.GetByHeroId(heroId);
        var titlesDto = _mapper.Map<IEnumerable<TitleDto>>(titles);
        var place = await GetHeroPlace(rating);
        var playStyle = await _unitOfWork.PlayStyles.GetByIdAsync(heroId);

        heroDto.PlayStyle = _mapper.Map<PlayStyleDto>(playStyle) ?? PlayStyleDto.Default(heroId);
        var statistics = new HeroStats
            {
                Hero = heroDto,
                HeroId = hero.Id,
                Points = points,
                Place = place,
                TotalMatches = fights.Count,
                TotalWins = fights.Count(x => x.IsWinner),
                TotalLooses = fights.Count(x => x.IsWinner == false),
                LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0,
                Titles = titlesDto
            };
        
        return statistics;
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
