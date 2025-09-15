namespace Unmatched.StatisticsService.Domain.Services;

using Unmatched.StatisticsService.Domain.Models;

public interface IHeroStatisticsService
{
    Task<IEnumerable<HeroStats>> GetHeroesStatisticsAsync();

    Task<HeroStats> GetHeroStatisticsAsync(Guid heroId);
}
