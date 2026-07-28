namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using Unmatched.StatisticsService.Domain.Models;

public class HeroPlaceAdjuster : IHeroPlaceAdjuster
{
    public IEnumerable<HeroStats> Adjust(IEnumerable<HeroStats> heroStats)
    {
        var place = 1;
        foreach (var stats in heroStats.OrderByDescending(x => x.Points).ThenByDescending(x => x.Kd).ThenByDescending(x => x.TotalMatches))
        {
            // Heroes with no recorded matches would otherwise clutter the top/bottom of the
            // ranking with ties on default stats, so they are left unranked instead.
            stats.Place = stats.TotalMatches > 0 ? place++ : null;
        }

        return heroStats;
    }
}
