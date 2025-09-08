namespace Unmatched.StatisticsService.Domain.Services;

public class MinionStatisticsService : IMinionStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MinionStatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<MinionStatisticsDto>> GetMinionsStatisticsAsync()
    {
        var minions = await _unitOfWork.Minions.GetAsync();
        // var ratings = await _unitOfWork.Ratings.GetAsync();
        // var fighters = await _unitOfWork.Fighters.GetFromFinishedMatchesAsync();

        var statistics = new List<MinionStatisticsDto>();

        foreach (var minion in minions)
        {
            var minionDto = _mapper.Map<MinionDto>(minion);
            // var points = ratings.FirstOrDefault(x => x.HeroId.Equals(hero.Id))?.Points ?? 0;
            // var fights = fighters.Where(x => x.HeroId.Equals(hero.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var minionStatistics = new MinionStatisticsDto 
                {
                    Minion = minionDto,
                    MinionId = minionDto.Id,
                    // Points = points,
                    // TotalMatches = fights.Length,
                    // TotalWins = fights.Count(x => x.IsWinner),
                    // TotalLooses = fights.Count(x => x.IsWinner == false),
                    // LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0
                };

            statistics.Add(minionStatistics);
        }

        return statistics;
    }

    public async Task<MinionStatisticsDto> GetMinionStatisticsAsync(Guid minionId)
    {
        var minion = await _unitOfWork.Minions.GetByIdAsync(minionId);
        var minionDto = _mapper.Map<MinionDto>(minion);
                 // var fights = await _unitOfWork.Fighters.GetFromFinishedMatchesByHeroIdAsync(heroId);
                 // var rating = await _unitOfWork.Ratings.GetByHeroIdAsync(hero.Id);
                 // var points = rating?.Points ?? 0;
                 // var titles = await _unitOfWork.Titles.GetByHeroId(heroId);
                 // var titlesDto = _mapper.Map<IEnumerable<TitleDto>>(titles);
                 // var place = await GetHeroPlace(rating);
                
        var statistics = new MinionStatisticsDto 
            {
                Minion = minionDto,
                MinionId = minionDto.Id,
                                // Points = points,
                                // Place = place,
                                // TotalMatches = fights.Count,
                                // TotalWins = fights.Count(x => x.IsWinner),
                                // TotalLooses = fights.Count(x => x.IsWinner == false),
                                // LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0,
                                // Titles = titlesDto
            };
                        
        return statistics;
    }
}
