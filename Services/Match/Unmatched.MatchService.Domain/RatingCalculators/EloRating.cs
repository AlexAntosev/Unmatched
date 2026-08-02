namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Constants;

/// <summary>
/// The Elo primitives shared by every calculator. Kept free of any Match/Fighter knowledge so it is
/// trivial to unit test and to reuse for any future rated subject (players, villains, ...).
/// </summary>
public static class EloRating
{
    /// <summary>Probability that <paramref name="rating"/> beats <paramref name="opponentRating"/>,
    /// "on paper" - before margin-of-victory is taken into account.</summary>
    public static double ExpectedScore(int rating, int opponentRating)
        => 1.0 / (1.0 + Math.Pow(10, (opponentRating - rating) / (double)RatingConstants.EloScale));

    /// <summary>The points a win moves the winner's rating by (the loser moves by exactly
    /// <c>-Delta</c>, so the pair stays zero-sum). <paramref name="performanceModifier"/> scales the
    /// result by how convincing the win was - see <see cref="PerformanceModifier"/>.</summary>
    public static int Delta(int winnerRating, int loserRating, double performanceModifier)
    {
        var expectedWinScore = ExpectedScore(winnerRating, loserRating);
        return (int)Math.Round(RatingConstants.KFactor * performanceModifier * (1 - expectedWinScore), MidpointRounding.AwayFromZero);
    }
}
