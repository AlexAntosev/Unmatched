namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using Unmatched.StatisticsService.Domain.Models;

public interface IHeroPlaceAdjuster
{
    IEnumerable<HeroStats> Adjust(IEnumerable<HeroStats> heroStats);
}