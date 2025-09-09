namespace Unmatched.StatisticsService.Domain.Services;

public class VillainStatisticsService : IVillainStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public VillainStatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VillainStatisticsDto>> GetVillainsStatisticsAsync()
    {
        var villains = await _unitOfWork.Villains.GetAsync();
        var ratings = await _unitOfWork.Ratings.GetAsync();
        var fighters = await _unitOfWork.Fighters.GetFromFinishedMatchesAsync();

        var statistics = new List<VillainStatisticsDto>();

        foreach (var villain in villains)
        {
            var villainDto = _mapper.Map<VillainDto>(villain);
            var points = ratings.FirstOrDefault(x => x.HeroId.Equals(hero.Id))?.Points ?? 0;
            var fights = fighters.Where(x => x.HeroId.Equals(hero.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var villainStatistics = new VillainStatisticsDto
            {
                Villain = villainDto,
                VillainId = villainDto.Id,
                Points = points,
                TotalMatches = fights.Length,
                TotalWins = fights.Count(x => x.IsWinner),
                TotalLooses = fights.Count(x => x.IsWinner == false),
                LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0
            };

            statistics.Add(villainStatistics);
        }

        return statistics;
    }

    public async Task<VillainStatisticsDto> GetVillainStatisticsAsync(Guid villainId)
    {
        var villain = await _unitOfWork.Villains.GetByIdAsync(villainId);
        var villainDto = _mapper.Map<VillainDto>(villain);
        var fights = await _unitOfWork.Fighters.GetFromFinishedMatchesByHeroIdAsync(heroId);
        var rating = await _unitOfWork.Ratings.GetByHeroIdAsync(hero.Id);
        var points = rating?.Points ?? 0;
        var titles = await _unitOfWork.Titles.GetByHeroId(heroId);
        var titlesDto = _mapper.Map<IEnumerable<TitleDto>>(titles);
        var place = await GetHeroPlace(rating);

        var statistics = new VillainStatisticsDto
        {
            Villain = villainDto,
            VillainId = villainDto.Id,
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
}
