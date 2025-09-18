namespace Unmatched.StatisticsService.Domain.Services.Contracts;

using Unmatched.StatisticsService.Domain.Models;

public interface IHeroStatisticsService
{
    Task<IEnumerable<HeroStats>> GetHeroesStatisticsAsync();

    Task<HeroStats> GetHeroStatisticsAsync(Guid heroId);
}