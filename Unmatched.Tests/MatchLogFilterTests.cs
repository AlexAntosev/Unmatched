namespace Unmatched.Tests;

using Unmatched.Dtos;
using Unmatched.Enums;
using Unmatched.Services.Statistics;

using static Unmatched.Tests.MatchLogBuilder;

public class MatchLogFilterTests
{
    private static readonly UiHeroDto Medusa = Hero("Medusa");
    private static readonly UiHeroDto Bigfoot = Hero("Bigfoot");
    private static readonly UiPlayerDto Ksuha = Player("Ksuha");
    private static readonly UiPlayerDto Andrii = Player("Andrii");

    private static readonly UiMatchLogDto[] Log =
    [
        Match(new DateTime(2026, 7, 12), [Fighter(Medusa, true, Ksuha), Fighter(Bigfoot, false, Andrii)],
            GameMode.OneVsOne, map: "Kaer Morhen", tournament: "Golden Halat League"),
        Match(new DateTime(2026, 7, 5), [Fighter(Bigfoot, true, Andrii), Fighter(Medusa, false, Ksuha)],
            GameMode.TeamVsTeam, map: "Castle", tournament: "Silverhand Tournament"),
        Match(new DateTime(2026, 6, 1), [Fighter(Medusa, true, Andrii), Fighter(Bigfoot, false, Ksuha)],
            GameMode.Cooperative, map: "London", tournament: "Golden Halat League")
    ];

    [Fact]
    public void Apply_NoCriteria_ReturnsEverything()
    {
        var filter = new MatchLogFilter();

        Assert.True(filter.IsEmpty);
        Assert.Equal(3, filter.Apply(Log).Count());
    }

    [Fact]
    public void Apply_Mode_KeepsOnlyThatMode()
    {
        var result = new MatchLogFilter { Mode = GameMode.TeamVsTeam }.Apply(Log);

        Assert.Equal(GameMode.TeamVsTeam, Assert.Single(result).GameMode);
    }

    [Fact]
    public void Apply_Player_MatchesEitherSide()
    {
        Assert.Equal(3, new MatchLogFilter { PlayerIds = [Ksuha.Id] }.Apply(Log).Count());
    }

    [Fact]
    public void Apply_MultiplePlayers_MatchesAnyOfThem()
    {
        // Every match in the fixture has either Ksuha or Andrii, so selecting both is a no-op filter.
        Assert.Equal(3, new MatchLogFilter { PlayerIds = [Ksuha.Id, Andrii.Id] }.Apply(Log).Count());
    }

    [Fact]
    public void Apply_Map_MatchesExactName()
    {
        var result = Assert.Single(new MatchLogFilter { MapNames = ["Castle"] }.Apply(Log));

        Assert.Equal("Castle", result.MapName);
    }

    [Fact]
    public void Apply_MultipleMaps_MatchesAnyOfThem()
    {
        Assert.Equal(2, new MatchLogFilter { MapNames = ["Castle", "London"] }.Apply(Log).Count());
    }

    [Fact]
    public void Apply_Tournament_MatchesExactName()
    {
        Assert.Equal(2, new MatchLogFilter { TournamentNames = ["Golden Halat League"] }.Apply(Log).Count());
    }

    [Fact]
    public void Apply_RangeDays_KeepsTheLoosestSelectedWindow()
    {
        // Selecting both a 5-day and a 60-day preset should behave like the 60-day one alone -
        // OR'ing date windows together is equivalent to keeping only the widest.
        var recent = Match(DateTime.Today.AddDays(-3), [Fighter(Medusa, true, Ksuha), Fighter(Bigfoot, false, Andrii)],
            GameMode.OneVsOne);
        var old = Match(DateTime.Today.AddDays(-40), [Fighter(Medusa, true, Ksuha), Fighter(Bigfoot, false, Andrii)],
            GameMode.OneVsOne);

        var filter = new MatchLogFilter { RangeDays = [5, 60] };

        Assert.Equal(2, filter.Apply([recent, old]).Count());
        Assert.Single(new MatchLogFilter { RangeDays = [5] }.Apply([recent, old]));
    }

    [Fact]
    public void Apply_Search_IsCaseInsensitiveAcrossHeroPlayerAndMap()
    {
        Assert.Single(new MatchLogFilter { Search = "kaer" }.Apply(Log));
        Assert.Equal(3, new MatchLogFilter { Search = "medusa" }.Apply(Log).Count());
        Assert.Equal(3, new MatchLogFilter { Search = "ANDRII" }.Apply(Log).Count());
    }

    [Fact]
    public void Apply_Search_NoMatch_ReturnsNothing()
    {
        Assert.Empty(new MatchLogFilter { Search = "sherlock" }.Apply(Log));
    }

    [Fact]
    public void Apply_CriteriaCombine()
    {
        var filter = new MatchLogFilter { Mode = GameMode.OneVsOne, PlayerIds = [Ksuha.Id], Search = "Kaer" };

        Assert.Single(filter.Apply(Log));
    }

    [Fact]
    public void IsEmpty_IsFalseOnceAnyCriterionIsSet()
    {
        Assert.False(new MatchLogFilter { Search = "x" }.IsEmpty);
        Assert.False(new MatchLogFilter { Mode = GameMode.FreeForAll }.IsEmpty);
        Assert.False(new MatchLogFilter { RangeDays = [30] }.IsEmpty);
    }

    [Fact]
    public void Clear_ResetsEveryCriterion()
    {
        var filter = new MatchLogFilter
        {
            Mode = GameMode.OneVsOne,
            PlayerIds = [Ksuha.Id],
            HeroIds = [Medusa.Id],
            MapNames = ["Castle"],
            TournamentNames = ["Golden Halat League"],
            RangeDays = [30]
        };

        filter.Clear();

        Assert.True(filter.IsEmpty);
    }
}
