namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.RatingCalculators;

public class EloRatingTests
{
    [Fact]
    public void ExpectedScore_EqualRatings_ReturnsHalf()
    {
        var expected = EloRating.ExpectedScore(1000, 1000);

        Assert.Equal(0.5, expected, precision: 10);
    }

    [Fact]
    public void ExpectedScore_HigherRating_ReturnsMoreThanHalf()
    {
        var expected = EloRating.ExpectedScore(1200, 1000);

        Assert.True(expected > 0.5);
    }

    [Fact]
    public void ExpectedScore_IsComplementaryFromEitherSide()
    {
        var fromA = EloRating.ExpectedScore(1200, 900);
        var fromB = EloRating.ExpectedScore(900, 1200);

        Assert.Equal(1.0, fromA + fromB, precision: 10);
    }

    [Fact]
    public void Delta_EqualRatings_ReturnsHalfOfK()
    {
        var delta = EloRating.Delta(winnerRating: 1000, loserRating: 1000, performanceModifier: 1.0);

        Assert.Equal(RatingConstants.KFactor / 2, delta);
    }

    [Fact]
    public void Delta_UnderdogWins_PaysMoreThanFavouriteWinning()
    {
        var underdogWinDelta = EloRating.Delta(winnerRating: 800, loserRating: 1200, performanceModifier: 1.0);
        var favouriteWinDelta = EloRating.Delta(winnerRating: 1200, loserRating: 800, performanceModifier: 1.0);

        Assert.True(underdogWinDelta > favouriteWinDelta);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(200)]
    [InlineData(300)]
    [InlineData(400)]
    public void Delta_IsMonotonicallyIncreasing_AsTheWinnerIsMoreOfAnUnderdog(int gap)
    {
        var smallerGapDelta = EloRating.Delta(winnerRating: 1000 - gap, loserRating: 1000, performanceModifier: 1.0);
        var largerGapDelta = EloRating.Delta(winnerRating: 1000 - gap - 50, loserRating: 1000, performanceModifier: 1.0);

        Assert.True(largerGapDelta >= smallerGapDelta);
    }

    [Fact]
    public void Delta_ScalesLinearlyWithPerformanceModifier()
    {
        var atBaseline = EloRating.Delta(winnerRating: 1000, loserRating: 1000, performanceModifier: 1.0);
        var atMaximum = EloRating.Delta(winnerRating: 1000, loserRating: 1000, performanceModifier: 1.25);

        Assert.Equal((int)Math.Round(atBaseline * 1.25), atMaximum);
    }
}
