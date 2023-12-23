namespace Unmatched.Services.Statistics;

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
            var points = ratings.FirstOrDefault(x => x.HeroId.Equals(hero.Id))?.Points ?? 0;
            var fights = fighters.Where(x => x.HeroId.Equals(hero.Id)).OrderByDescending(x => x.Match.Date).ToArray();

            var heroStatistics = new HeroStatisticsDto 
                {
                    HeroId = hero.Id,
                    HeroName = hero.Name,
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
    
    public async Task<HeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId)
    {
        var hero = await _unitOfWork.Heroes.GetByIdAsync(heroId);
        var fights = await _unitOfWork.Fighters
            .Query()
            .Include(x => x.Match)
            .Where(x => x.HeroId.Equals(hero.Id))
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
        
        var statistics = new HeroStatisticsDto
            {
                HeroId = hero.Id,
                HeroName = hero.Name,
                TotalMatches = fights.Count,
                TotalWins = fights.Count(x => x.IsWinner),
                TotalLooses = fights.Count(x => x.IsWinner == false),
                LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0
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
}
