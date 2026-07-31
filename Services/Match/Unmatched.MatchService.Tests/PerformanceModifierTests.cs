namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;

public class PerformanceModifierTests
{
    private static readonly CatalogHeroDto HeroWithSidekick = new()
    {
        Hp = 20, DeckSize = 10, Sidekicks = new[] { new CatalogSidekickDto { Hp = 10, Count = 1 } }
    };

    private static readonly CatalogHeroDto HeroWithoutSidekick = new()
    {
        Hp = 20, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>()
    };

    [Fact]
    public void Calculate_TotalCrush_ReturnsUpperBound()
    {
        var winner = new FighterEntity { HpLeft = 20, SidekickHpLeft = 10, CardsLeft = 10 };
        var loser = new FighterEntity { HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 };

        var m = PerformanceModifier.Calculate(
            [PerformanceModifier.From(winner, HeroWithSidekick)],
            [PerformanceModifier.From(loser, HeroWithSidekick)]);

        Assert.Equal(1.25, m, precision: 10);
    }

    [Fact]
    public void Calculate_WorstPossibleWin_ReturnsLowerBound()
    {
        // hypothetical: winner kept nothing, loser kept everything but their own HP. Doesn't happen in
        // a real match (the winner must reduce the loser's HP to 0), but it pins down the formula's floor.
        var winner = new FighterEntity { HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 };
        var loser = new FighterEntity { HpLeft = 0, SidekickHpLeft = 10, CardsLeft = 10 };

        var m = PerformanceModifier.Calculate(
            [PerformanceModifier.From(winner, HeroWithSidekick)],
            [PerformanceModifier.From(loser, HeroWithSidekick)]);

        Assert.Equal(0.75, m, precision: 10);
    }

    [Fact]
    public void Calculate_BothSidesMissingAllStatData_ReturnsExactlyNeutral()
    {
        var winner = new FighterEntity { HpLeft = null, SidekickHpLeft = null, CardsLeft = null };
        var loser = new FighterEntity { HpLeft = null, SidekickHpLeft = null, CardsLeft = null };

        var m = PerformanceModifier.Calculate(
            [PerformanceModifier.From(winner, HeroWithSidekick)],
            [PerformanceModifier.From(loser, HeroWithSidekick)]);

        Assert.Equal(1.0, m, precision: 10);
    }

    [Fact]
    public void Calculate_WinnerMissingHp_ThatSideScoresNeutralRatherThanThrowing()
    {
        var winnerMissingHp = new FighterEntity { HpLeft = null, SidekickHpLeft = 10, CardsLeft = 10 };
        var loser = new FighterEntity { HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 };

        var m = PerformanceModifier.Calculate(
            [PerformanceModifier.From(winnerMissingHp, HeroWithSidekick)],
            [PerformanceModifier.From(loser, HeroWithSidekick)]);

        // ownScore neutral (0.5) because HpLeft is missing; oppScore is a full wipe (0)
        var expected = 0.75 + 0.5 * (0.5 * 0.5 + 0.5 * (1 - 0));
        Assert.Equal(expected, m, precision: 10);
    }

    [Fact]
    public void Calculate_EmptyLosingSide_ScoresTheSameAsMissingData()
    {
        // this is exactly the free-for-all case: there is no single "loser" to compare the winner against
        var winner = new FighterEntity { HpLeft = 20, SidekickHpLeft = 10, CardsLeft = 10 };

        var m = PerformanceModifier.Calculate([PerformanceModifier.From(winner, HeroWithSidekick)], []);

        var expected = 0.75 + 0.5 * (0.5 * 1.0 + 0.5 * (1 - 0.5));
        Assert.Equal(expected, m, precision: 10);
    }

    [Fact]
    public void Calculate_HeroWithoutSidekicks_RedistributesTheDroppedWeight()
    {
        var winner = new FighterEntity { HpLeft = 20, SidekickHpLeft = null, CardsLeft = 10 };
        var loser = new FighterEntity { HpLeft = 0, SidekickHpLeft = null, CardsLeft = 0 };

        var m = PerformanceModifier.Calculate(
            [PerformanceModifier.From(winner, HeroWithoutSidekick)],
            [PerformanceModifier.From(loser, HeroWithoutSidekick)]);

        // full marks despite the null sidekick field - the hero simply has none, so it isn't "missing data"
        Assert.Equal(1.25, m, precision: 10);
    }

    [Fact]
    public void Calculate_CleanWin_ScoresHigherThanASqueakyWin()
    {
        var clean = new FighterEntity { HpLeft = 18, SidekickHpLeft = 9, CardsLeft = 9 };
        var squeak = new FighterEntity { HpLeft = 1, SidekickHpLeft = 1, CardsLeft = 1 };
        var loser = new FighterEntity { HpLeft = 0, SidekickHpLeft = 2, CardsLeft = 3 };

        var cleanM = PerformanceModifier.Calculate([PerformanceModifier.From(clean, HeroWithSidekick)], [PerformanceModifier.From(loser, HeroWithSidekick)]);
        var squeakM = PerformanceModifier.Calculate([PerformanceModifier.From(squeak, HeroWithSidekick)], [PerformanceModifier.From(loser, HeroWithSidekick)]);

        Assert.True(cleanM > squeakM);
    }

    [Fact]
    public void Calculate_MultiFighterSide_AveragesOverMembers()
    {
        var fullHealth = new FighterEntity { HpLeft = 20, SidekickHpLeft = 10, CardsLeft = 10 };
        var wiped = new FighterEntity { HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 };
        var loser = new FighterEntity { HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 };

        var mixedTeamM = PerformanceModifier.Calculate(
            [PerformanceModifier.From(fullHealth, HeroWithSidekick), PerformanceModifier.From(wiped, HeroWithSidekick)],
            [PerformanceModifier.From(loser, HeroWithSidekick)]);
        var fullTeamM = PerformanceModifier.Calculate(
            [PerformanceModifier.From(fullHealth, HeroWithSidekick), PerformanceModifier.From(fullHealth, HeroWithSidekick)],
            [PerformanceModifier.From(loser, HeroWithSidekick)]);

        Assert.True(mixedTeamM < fullTeamM);
    }
}
