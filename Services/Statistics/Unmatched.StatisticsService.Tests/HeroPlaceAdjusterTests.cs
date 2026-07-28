namespace Unmatched.StatisticsService.Tests;

using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Models;

public class HeroPlaceAdjusterTests
{
    private readonly HeroPlaceAdjuster _adjuster = new();

    [Fact]
    public void Adjust_HeroesWithNoMatches_GetNullPlace()
    {
        var unplayed = new HeroStats { HeroId = Guid.NewGuid(), Points = 0, TotalMatches = 0, TotalWins = 0, TotalLooses = 0 };

        _adjuster.Adjust([unplayed]);

        Assert.Null(unplayed.Place);
    }

    [Fact]
    public void Adjust_PlayedHeroes_GetContiguousPlacesIgnoringUnplayedHeroes()
    {
        var first = new HeroStats { HeroId = Guid.NewGuid(), Points = 10, TotalMatches = 5, TotalWins = 4, TotalLooses = 1 };
        var unplayed = new HeroStats { HeroId = Guid.NewGuid(), Points = 0, TotalMatches = 0, TotalWins = 0, TotalLooses = 0 };
        var second = new HeroStats { HeroId = Guid.NewGuid(), Points = 5, TotalMatches = 3, TotalWins = 1, TotalLooses = 2 };

        _adjuster.Adjust([first, unplayed, second]);

        Assert.Equal(1, first.Place);
        Assert.Null(unplayed.Place);
        Assert.Equal(2, second.Place);
    }

    [Fact]
    public void Adjust_OrdersByPointsThenKdThenTotalMatches()
    {
        var lowerPoints = new HeroStats { HeroId = Guid.NewGuid(), Points = 5, TotalMatches = 10, TotalWins = 5, TotalLooses = 5 };
        var higherPoints = new HeroStats { HeroId = Guid.NewGuid(), Points = 10, TotalMatches = 2, TotalWins = 1, TotalLooses = 1 };

        _adjuster.Adjust([lowerPoints, higherPoints]);

        Assert.Equal(1, higherPoints.Place);
        Assert.Equal(2, lowerPoints.Place);
    }
}
