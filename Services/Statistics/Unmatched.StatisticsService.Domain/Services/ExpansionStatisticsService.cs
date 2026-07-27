namespace Unmatched.StatisticsService.Domain.Services;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.Domain.Services.Contracts;

/// <summary>
/// Rolls per-hero stats up to the box the hero ships in, which is what the Collection screen shows
/// next to each expansion. The hero -> expansion link only exists in the catalog, so it is read
/// from the catalog cache and joined here.
/// </summary>
public class ExpansionStatisticsService(ICatalogHeroCache catalogHeroCache, IUnitOfWork unitOfWork)
    : IExpansionStatisticsService
{
    public async Task<IEnumerable<ExpansionStats>> GetExpansionsStatisticsAsync()
    {
        var heroes = await catalogHeroCache.GetAsync();
        var statsByHero = (await unitOfWork.HeroStats.GetAllAsync()).ToDictionary(s => s.HeroId);

        return heroes
            .Where(hero => hero.ExpansionId is not null)
            .GroupBy(hero => hero.ExpansionId!.Value)
            .Select(group => Aggregate(group.Key, group.Select(hero => statsByHero.GetValueOrDefault(hero.Id)).OfType<HeroStats>()))
            .ToList();
    }

    private static ExpansionStats Aggregate(Guid expansionId, IEnumerable<HeroStats> heroStats)
    {
        var stats = heroStats.ToList();

        // "Best hero" is the box's headline act: most points, and among equals the one that wins more.
        var best = stats
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.TotalWins)
            .FirstOrDefault();

        return new ExpansionStats
            {
                ExpansionId = expansionId,
                TotalMatches = stats.Sum(s => s.TotalMatches),
                TotalWins = stats.Sum(s => s.TotalWins),
                TotalLooses = stats.Sum(s => s.TotalLooses),
                BestHeroId = best?.HeroId,
                BestHeroName = best?.Name
            };
    }
}
