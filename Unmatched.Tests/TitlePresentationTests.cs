namespace Unmatched.Tests;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;
using Unmatched.Enums;
using Unmatched.Services.Statistics;

public class TitlePresentationTests
{
    private static readonly Guid HeroId = Guid.NewGuid();
    private static readonly Guid OtherHeroId = Guid.NewGuid();
    private static readonly Guid TournamentId = Guid.NewGuid();

    private static TitleDto Title(
        string name,
        TitleExclusivity exclusivity,
        string? ruleKey = null,
        Guid? tournamentId = null,
        TournamentTitleKind? kind = null,
        params TitleHolderDto[] holders)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Comment = $"{name} description.",
            Exclusivity = exclusivity,
            RuleKey = ruleKey,
            TournamentId = tournamentId,
            Kind = kind,
            Holders = holders
        };

    private static TitleHolderDto Holder(Guid heroId, int timesEarned = 1, DateTime? earnedAt = null, double? metric = null)
        => new() { HeroId = heroId, TimesEarned = timesEarned, EarnedAt = earnedAt, Metric = metric };

    private static TournamentDto Tournament(Guid id, string name, DateTime? completedAt = null)
        => new() { Id = id, Name = name, CompletedAt = completedAt };

    [Fact]
    public void Catalog_BucketsBySharedUniqueAndTournamentScope()
    {
        var achievement = Title("Flawless", TitleExclusivity.Shared, ruleKey: "flawless");
        var uniqueTitle = Title("The Streak", TitleExclusivity.Unique, ruleKey: "streak");
        var tournamentTitle = Title("Champion of Cup", TitleExclusivity.Unique, tournamentId: TournamentId, kind: TournamentTitleKind.Champion);
        var tournaments = new[] { Tournament(TournamentId, "Cup") };

        var catalog = TitlePresentation.Catalog([achievement, uniqueTitle, tournamentTitle], tournaments);

        Assert.Single(catalog.Achievements);
        Assert.Equal("Flawless", catalog.Achievements[0].Entry.Title.Name);
        Assert.Single(catalog.Titles);
        Assert.Equal("The Streak", catalog.Titles[0].Entry.Title.Name);
        Assert.Single(catalog.TournamentGroups);
        Assert.Single(catalog.TournamentGroups[0].Rows);
        Assert.Equal("Champion of Cup", catalog.TournamentGroups[0].Rows[0].Entry.Title.Name);
    }

    [Fact]
    public void Catalog_VacantUniqueTitle_HasNullHolder()
    {
        var vacant = Title("Bounty Holder", TitleExclusivity.Unique, ruleKey: "bounty-holder");

        var catalog = TitlePresentation.Catalog([vacant], []);

        Assert.Null(catalog.Titles[0].Holder);
        Assert.Equal(0, catalog.TitlesHeldCount);
        Assert.Equal(1, catalog.TitlesVacantCount);
    }

    [Fact]
    public void Catalog_HeldUniqueTitle_CarriesTheHolder()
    {
        var held = Title("The Streak", TitleExclusivity.Unique, ruleKey: "streak", holders: Holder(HeroId));

        var catalog = TitlePresentation.Catalog([held], []);

        Assert.Equal(HeroId, catalog.Titles[0].Holder!.HeroId);
        Assert.Equal(1, catalog.TitlesHeldCount);
        Assert.Equal(0, catalog.TitlesVacantCount);
    }

    [Fact]
    public void Catalog_Achievement_TopHoldersOrderedByTimesEarnedThenMostRecent()
    {
        var achievement = Title(
            "Flawless",
            TitleExclusivity.Shared,
            ruleKey: "flawless",
            holders:
            [
                Holder(HeroId, timesEarned: 2, earnedAt: new DateTime(2026, 1, 1)),
                Holder(OtherHeroId, timesEarned: 5, earnedAt: new DateTime(2026, 2, 1))
            ]);

        var catalog = TitlePresentation.Catalog([achievement], []);

        var card = catalog.Achievements[0];
        Assert.Equal(OtherHeroId, card.TopHolders[0].HeroId);
        Assert.Equal(2, card.TotalHolderCount);
        Assert.Equal(7, card.TotalAwards);
    }

    [Fact]
    public void Catalog_TournamentGroups_OrderedByKindWithinEachTournament()
    {
        var runnerUp = Title("Runner-Up of Cup", TitleExclusivity.Unique, tournamentId: TournamentId, kind: TournamentTitleKind.RunnerUp);
        var champion = Title("Champion of Cup", TitleExclusivity.Unique, tournamentId: TournamentId, kind: TournamentTitleKind.Champion);
        var tournaments = new[] { Tournament(TournamentId, "Cup") };

        var catalog = TitlePresentation.Catalog([runnerUp, champion], tournaments);

        var rows = catalog.TournamentGroups[0].Rows;
        Assert.Equal("Champion of Cup", rows[0].Entry.Title.Name);
        Assert.Equal("Runner-Up of Cup", rows[1].Entry.Title.Name);
    }

    [Fact]
    public void Catalog_TournamentTitleWithoutAMatchingTournament_IsDropped()
    {
        var orphaned = Title("Champion of Deleted Cup", TitleExclusivity.Unique, tournamentId: TournamentId, kind: TournamentTitleKind.Champion);

        var catalog = TitlePresentation.Catalog([orphaned], []);

        Assert.Empty(catalog.TournamentGroups);
    }

    [Fact]
    public void ForHero_OnlyIncludesWhatTheHeroHolds()
    {
        var heldTitle = Title("The Streak", TitleExclusivity.Unique, ruleKey: "streak", holders: Holder(HeroId));
        var otherTitle = Title("Grand Champion", TitleExclusivity.Unique, ruleKey: "grand-champion", holders: Holder(OtherHeroId));
        var heldAchievement = Title("Flawless", TitleExclusivity.Shared, ruleKey: "flawless", holders: Holder(HeroId, timesEarned: 3));
        var unearnedAchievement = Title("Rusher", TitleExclusivity.Shared, ruleKey: "rusher");

        var summary = TitlePresentation.ForHero(HeroId, [heldTitle, otherTitle, heldAchievement, unearnedAchievement], []);

        Assert.Single(summary.HeldTitles);
        Assert.Equal("The Streak", summary.HeldTitles[0].Entry.Title.Name);
        Assert.Single(summary.HeldAchievements);
        Assert.Equal(3, summary.HeldAchievements[0].TimesEarned);
        Assert.Equal(1, summary.TitlesHeldCount);
        Assert.Equal(1, summary.AchievementsHeldCount);
        Assert.Equal(2, summary.AchievementsTotalCount);
    }

    [Fact]
    public void ForHero_TournamentTitle_JoinsTheTournamentName()
    {
        var earnedAt = new DateTime(2026, 2, 14);
        var championTitle = Title(
            "Champion of Winter Cup",
            TitleExclusivity.Unique,
            tournamentId: TournamentId,
            kind: TournamentTitleKind.Champion,
            holders: Holder(HeroId, earnedAt: earnedAt));
        var tournaments = new[] { Tournament(TournamentId, "Winter Cup", earnedAt) };

        var summary = TitlePresentation.ForHero(HeroId, [championTitle], tournaments);

        Assert.Single(summary.TournamentTitles);
        Assert.Equal("Winter Cup", summary.TournamentTitles[0].TournamentName);
        Assert.Equal(earnedAt, summary.TournamentTitles[0].EarnedAt);
    }

    [Theory]
    [InlineData("flawless", TitleAccent.Achievement)]
    [InlineData("grand-champion", TitleAccent.Prestige)]
    [InlineData("sufferer", TitleAccent.Negative)]
    [InlineData("wall", TitleAccent.Neutral)]
    public void TitleStyle_KnownRuleKey_ReturnsItsAccent(string ruleKey, TitleAccent expectedAccent)
    {
        var title = Title("Some Title", TitleExclusivity.Shared, ruleKey: ruleKey);

        Assert.Equal(expectedAccent, TitleStyle.For(title).Accent);
    }

    [Fact]
    public void TitleStyle_UnrecognizedRuleKey_FallsBackRatherThanThrowing()
    {
        var manualShared = Title("Custom Award", TitleExclusivity.Shared, ruleKey: "not-a-real-rule");
        var manualUnique = Title("Custom Title", TitleExclusivity.Unique);

        var sharedStyle = TitleStyle.For(manualShared);
        var uniqueStyle = TitleStyle.For(manualUnique);

        Assert.Equal(TitleAccent.Achievement, sharedStyle.Accent);
        Assert.Equal(TitleAccent.Neutral, uniqueStyle.Accent);
    }

    [Theory]
    [InlineData("executioner", 214, "214 sidekick HP")]
    [InlineData("workhorse", 128, "128 matches")]
    [InlineData("streak", 9, "9 wins in a row")]
    [InlineData("sufferer", 6, "6 losses in a row")]
    [InlineData("grand-champion", 1842, "1842 rating")]
    [InlineData("last-breath", 2, "2 HP left")]
    [InlineData("rusher", 23, "23 cards left")]
    [InlineData("punisher", 45, "+45 rating")]
    [InlineData("giant-slayer", 312, "312 rating gap")]
    public void TitleEntry_MetricLabel_FormatsWithTheRulesUnit(string ruleKey, double metric, string expected)
    {
        var title = Title("Some Title", TitleExclusivity.Shared, ruleKey: ruleKey);
        var entry = new TitleEntry(title, TitleStyle.For(title));

        Assert.Equal(expected, entry.MetricLabel(metric));
    }

    [Fact]
    public void TitleEntry_MetricLabel_UsesOneDecimalForWall()
    {
        var title = Title("The Wall", TitleExclusivity.Unique, ruleKey: "wall");
        var entry = new TitleEntry(title, TitleStyle.For(title));

        Assert.Equal("5.1 HP per win", entry.MetricLabel(5.1));
    }

    [Fact]
    public void TitleEntry_MetricLabel_NullMetric_ReturnsNull()
    {
        var title = Title("Some Title", TitleExclusivity.Shared, ruleKey: "executioner");
        var entry = new TitleEntry(title, TitleStyle.For(title));

        Assert.Null(entry.MetricLabel(null));
    }

    [Theory]
    [InlineData("flawless")]
    [InlineData("deck-miller")]
    [InlineData("kingslayer")]
    [InlineData("bounty-holder")]
    public void TitleEntry_MetricLabel_RuleWithNoNaturalMetric_ReturnsNullEvenIfAValueSomehowExists(string ruleKey)
    {
        var title = Title("Some Title", TitleExclusivity.Shared, ruleKey: ruleKey);
        var entry = new TitleEntry(title, TitleStyle.For(title));

        Assert.Null(entry.MetricLabel(42));
    }

    [Fact]
    public void ForHero_HeldAchievement_CarriesTheMetricLabel()
    {
        var achievement = Title("Executioner", TitleExclusivity.Shared, ruleKey: "executioner", holders: Holder(HeroId, metric: 214));

        var summary = TitlePresentation.ForHero(HeroId, [achievement], []);

        Assert.Equal("214 sidekick HP", summary.HeldAchievements[0].MetricLabel);
    }

    [Fact]
    public void ForHero_HeldTitle_CarriesTheMetricLabel()
    {
        var title = Title("The Streak", TitleExclusivity.Unique, ruleKey: "streak", holders: Holder(HeroId, metric: 9));

        var summary = TitlePresentation.ForHero(HeroId, [title], []);

        Assert.Equal("9 wins in a row", summary.HeldTitles[0].MetricLabel);
    }

    [Fact]
    public void TitleEntry_DisplayName_TournamentTitle_UsesBareKindLabelNotTheBakedInName()
    {
        var championTitle = Title("Champion of Winter Cup", TitleExclusivity.Unique, tournamentId: TournamentId, kind: TournamentTitleKind.Champion);
        var runnerUpTitle = Title("Runner-Up of Winter Cup", TitleExclusivity.Unique, tournamentId: TournamentId, kind: TournamentTitleKind.RunnerUp);
        var plainTitle = Title("The Streak", TitleExclusivity.Unique, ruleKey: "streak");

        Assert.Equal("Champion", new TitleEntry(championTitle, TitleStyle.For(championTitle)).DisplayName);
        Assert.Equal("Runner-Up", new TitleEntry(runnerUpTitle, TitleStyle.For(runnerUpTitle)).DisplayName);
        Assert.Equal("The Streak", new TitleEntry(plainTitle, TitleStyle.For(plainTitle)).DisplayName);
    }

    [Fact]
    public void TitleStyle_TournamentKind_TakesPrecedenceOverRuleKey()
    {
        var tournamentTitle = Title("Champion of Cup", TitleExclusivity.Unique, tournamentId: TournamentId, kind: TournamentTitleKind.Champion);

        Assert.Equal(TitleAccent.Prestige, TitleStyle.For(tournamentTitle).Accent);
        Assert.Equal("bi-trophy-fill", TitleStyle.For(tournamentTitle).Icon);
    }
}
