namespace Unmatched.Tests;

using Unmatched.Enums;
using Unmatched.Services.Statistics;

using static Unmatched.Tests.MatchLogBuilder;

public class HeroMatchupCalculatorTests
{
    [Fact]
    public void Calculate_SplitsWinsAndLossesPerOpponent()
    {
        var geralt = Hero("Geralt of Rivia");
        var alice = Hero("Alice");
        var dracula = Hero("Dracula");
        var matches = new[]
            {
                Duel(new DateTime(2026, 1, 1), geralt, alice, heroWon: true),
                Duel(new DateTime(2026, 1, 2), geralt, alice, heroWon: true),
                Duel(new DateTime(2026, 1, 3), geralt, dracula, heroWon: false)
            };

        var matchups = HeroMatchupCalculator.Calculate(geralt.Id, matches).ToDictionary(m => m.OpponentName);

        Assert.Equal(2, matchups["Alice"].Wins);
        Assert.Equal(0, matchups["Alice"].Losses);
        Assert.Equal(0, matchups["Dracula"].Wins);
        Assert.Equal(1, matchups["Dracula"].Losses);
    }

    [Fact]
    public void Calculate_WinPercentIsRoundedWholeNumber()
    {
        var geralt = Hero("Geralt of Rivia");
        var alice = Hero("Alice");
        var matches = new[]
            {
                Duel(new DateTime(2026, 1, 1), geralt, alice, heroWon: true),
                Duel(new DateTime(2026, 1, 2), geralt, alice, heroWon: true),
                Duel(new DateTime(2026, 1, 3), geralt, alice, heroWon: false)
            };

        Assert.Equal(67, HeroMatchupCalculator.Calculate(geralt.Id, matches).Single().WinPercent);
    }

    [Fact]
    public void Calculate_TeamMatesAreNotOpponents()
    {
        var geralt = Hero("Geralt of Rivia");
        var mate = Hero("Robin Hood");
        var enemy = Hero("Dracula");
        var otherEnemy = Hero("Alice");
        var match = Match(
            new DateTime(2026, 1, 1),
            [
                Fighter(geralt, true, team: 1), Fighter(mate, true, team: 1),
                Fighter(enemy, false, team: 2), Fighter(otherEnemy, false, team: 2)
            ],
            GameMode.TeamVsTeam);

        var opponents = HeroMatchupCalculator.Calculate(geralt.Id, [match]).Select(m => m.OpponentName).Order();

        Assert.Equal(["Alice", "Dracula"], opponents);
    }

    [Fact]
    public void Calculate_MatchesWithoutTheHero_AreIgnored()
    {
        var geralt = Hero("Geralt of Rivia");
        var alice = Hero("Alice");
        var dracula = Hero("Dracula");
        var matches = new[] { Duel(new DateTime(2026, 1, 1), alice, dracula, heroWon: true) };

        Assert.Empty(HeroMatchupCalculator.Calculate(geralt.Id, matches));
    }

    [Fact]
    public void Antagonist_IsTheMostFrequentOpponent()
    {
        var geralt = Hero("Geralt of Rivia");
        var alice = Hero("Alice");
        var arthur = Hero("King Arthur");
        var matches = new[]
            {
                Duel(new DateTime(2026, 1, 1), geralt, alice, heroWon: true),
                Duel(new DateTime(2026, 1, 2), geralt, arthur, heroWon: false),
                Duel(new DateTime(2026, 1, 3), geralt, arthur, heroWon: false),
                Duel(new DateTime(2026, 1, 4), geralt, arthur, heroWon: true)
            };

        var antagonist = HeroMatchupCalculator.Antagonist(HeroMatchupCalculator.Calculate(geralt.Id, matches));

        Assert.Equal("King Arthur", antagonist!.OpponentName);
        Assert.Equal(3, antagonist.Total);
    }

    [Fact]
    public void Antagonist_NoMatchesPlayed_IsNull()
    {
        Assert.Null(HeroMatchupCalculator.Antagonist([]));
    }

    [Fact]
    public void BestAndWorst_ShowsTheSpreadNotTheTopOfTheList()
    {
        var matchups = Enumerable.Range(0, 10)
            .Select(i => new HeroMatchup(Guid.NewGuid(), $"Hero {i}", "/Unknown.png", 10 - i, i))
            .OrderByDescending(m => m.WinPercent)
            .ToList();

        var shown = HeroMatchupCalculator.BestAndWorst(matchups);

        Assert.Equal(5, shown.Count);
        Assert.Equal(matchups.Take(3), shown.Take(3));
        Assert.Equal(matchups.TakeLast(2), shown.TakeLast(2));
    }

    [Fact]
    public void BestAndWorst_ShortListIsReturnedWhole()
    {
        var matchups = Enumerable.Range(0, 4)
            .Select(i => new HeroMatchup(Guid.NewGuid(), $"Hero {i}", "/Unknown.png", 4 - i, i))
            .ToList();

        Assert.Equal(4, HeroMatchupCalculator.BestAndWorst(matchups).Count);
    }

    [Fact]
    public void Kd_WithoutLosses_EqualsWins()
    {
        var geralt = Hero("Geralt of Rivia");
        var alice = Hero("Alice");
        var matches = new[]
            {
                Duel(new DateTime(2026, 1, 1), geralt, alice, heroWon: true),
                Duel(new DateTime(2026, 1, 2), geralt, alice, heroWon: true)
            };

        Assert.Equal(2, HeroMatchupCalculator.Calculate(geralt.Id, matches).Single().Kd);
    }
}
