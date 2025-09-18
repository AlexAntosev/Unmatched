namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using Unmatched.StatisticsService.Domain.Models;

public class HeroPlaceAdjuster : IHeroPlaceAdjuster
{
    public IEnumerable<HeroStats> Adjust(IEnumerable<HeroStats> heroStats)
    {
        var place = 1;
        foreach (var stats in heroStats.OrderByDescending(x => x.Points).ThenByDescending(x => x.Kd).ThenByDescending(x => x.TotalMatches))
        {
            stats.Place = place;
            place++;
        }

        return heroStats;
    }
}
