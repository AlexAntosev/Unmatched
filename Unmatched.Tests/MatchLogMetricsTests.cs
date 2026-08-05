namespace Unmatched.Tests;

using Unmatched.Services.Statistics;

using static Unmatched.Tests.MatchLogBuilder;

public class MatchLogMetricsTests
{
    private static readonly DateTime Today = new(2026, 7, 15);

    [Fact]
    public void Calculate_NoMatches_ReturnsEmpty()
    {
        Assert.Equal(MatchLogMetrics.Empty, MatchLogMetrics.Calculate([], Today));
    }

    [Fact]
    public void Calculate_CountsEveryMatch()
    {
        var alice = Hero("Alice");
        var bigfoot = Hero("Bigfoot");
        var matches = new[]
            {
                Duel(new DateTime(2024, 1, 1), alice, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 7, 14), alice, bigfoot, heroWon: false)
            };

        Assert.Equal(2, MatchLogMetrics.Calculate(matches, Today).TotalMatches);
    }

    [Fact]
    public void Calculate_AddedThisWeek_CountsOnlyTheLastSevenDays()
    {
        var alice = Hero("Alice");
        var bigfoot = Hero("Bigfoot");
        var matches = new[]
            {
                Duel(new DateTime(2026, 7, 14), alice, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 7, 10), alice, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 7, 1), alice, bigfoot, heroWon: true)
            };

        Assert.Equal(2, MatchLogMetrics.Calculate(matches, Today).AddedThisWeek);
    }

    [Fact]
    public void Calculate_SessionsThisMonth_CountsDistinctDaysNotMatches()
    {
        var alice = Hero("Alice");
        var bigfoot = Hero("Bigfoot");
        var matches = new[]
            {
                Duel(new DateTime(2026, 7, 5, 19, 0, 0), alice, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 7, 5, 21, 0, 0), alice, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 7, 9), alice, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 6, 30), alice, bigfoot, heroWon: true)
            };

        var metrics = MatchLogMetrics.Calculate(matches, Today);

        Assert.Equal(3, metrics.MatchesThisMonth);
        Assert.Equal(2, metrics.SessionsThisMonth);
    }

    [Fact]
    public void Calculate_AverageEpic_IgnoresUnratedMatches()
    {
        var alice = Hero("Alice");
        var bigfoot = Hero("Bigfoot");
        var matches = new[]
            {
                Duel(new DateTime(2026, 7, 1), alice, bigfoot, heroWon: true, epic: 3),
                Duel(new DateTime(2026, 7, 2), alice, bigfoot, heroWon: true, epic: 2),
                Duel(new DateTime(2026, 7, 3), alice, bigfoot, heroWon: true)
            };

        Assert.Equal(2.5, MatchLogMetrics.Calculate(matches, Today).AverageEpic);
    }

    [Fact]
    public void Calculate_LongestStreak_ResetsOnALoss()
    {
        var medusa = Hero("Medusa");
        var bigfoot = Hero("Bigfoot");
        var matches = new[]
            {
                Duel(new DateTime(2026, 6, 1), medusa, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 6, 2), medusa, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 6, 3), medusa, bigfoot, heroWon: false),
                Duel(new DateTime(2026, 6, 4), medusa, bigfoot, heroWon: true)
            };

        var streak = MatchLogMetrics.Calculate(matches, Today).LongestStreak;

        Assert.NotNull(streak);
        Assert.Equal("Medusa", streak!.HeroName);
        Assert.Equal(2, streak.Length);
    }

    [Fact]
    public void Calculate_LongestStreak_CaptionNamesHeroAndMonth()
    {
        var medusa = Hero("Medusa");
        var bigfoot = Hero("Bigfoot");
        var matches = new[] { Duel(new DateTime(2026, 6, 2), medusa, bigfoot, heroWon: true) };

        Assert.Equal("Medusa · 2 June 2026", MatchLogMetrics.Calculate(matches, Today).LongestStreak!.Caption);
    }

    [Fact]
    public void Calculate_LongestStreak_ComparesHeroesIndependently()
    {
        var medusa = Hero("Medusa");
        var alice = Hero("Alice");
        var bigfoot = Hero("Bigfoot");
        var matches = new[]
            {
                Duel(new DateTime(2026, 6, 1), medusa, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 6, 2), medusa, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 6, 3), alice, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 6, 4), alice, bigfoot, heroWon: true),
                Duel(new DateTime(2026, 6, 5), alice, bigfoot, heroWon: true)
            };

        var streak = MatchLogMetrics.Calculate(matches, Today).LongestStreak;

        Assert.Equal("Alice", streak!.HeroName);
        Assert.Equal(3, streak.Length);
    }

    [Fact]
    public void Calculate_EveryHeroAlwaysLoses_ReportsNoStreak()
    {
        var alice = Hero("Alice");
        var bigfoot = Hero("Bigfoot");
        // Both fighters cannot lose in a real duel, so build a match where nobody is marked winner.
        var matches = new[] { Match(new DateTime(2026, 6, 1), [Fighter(alice, false), Fighter(bigfoot, false)]) };

        Assert.Null(MatchLogMetrics.Calculate(matches, Today).LongestStreak);
    }
}
