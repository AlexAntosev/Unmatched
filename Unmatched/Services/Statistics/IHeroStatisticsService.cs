namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;

public interface IHeroStatisticsService
{
    Task<IEnumerable<HeroStatisticsDto>> GetHeroesStatisticsAsync();

    Task<HeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId);

    Task<IEnumerable<MatchLogDto>> GetHeroMatchesAsync(Guid heroId);
}
