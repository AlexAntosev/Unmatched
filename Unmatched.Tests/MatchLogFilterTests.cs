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
        Assert.Equal(3, new MatchLogFilter { PlayerId = Ksuha.Id }.Apply(Log).Count());
    }

    [Fact]
    public void Apply_Map_MatchesExactName()
    {
        var result = Assert.Single(new MatchLogFilter { MapName = "Castle" }.Apply(Log));

        Assert.Equal("Castle", result.MapName);
    }

    [Fact]
    public void Apply_Tournament_MatchesExactName()
    {
        Assert.Equal(2, new MatchLogFilter { TournamentName = "Golden Halat League" }.Apply(Log).Count());
    }

    [Fact]
    public void Apply_DateRange_IsInclusive()
    {
        var filter = new MatchLogFilter { From = new DateTime(2026, 7, 5), To = new DateTime(2026, 7, 12) };

        Assert.Equal(2, filter.Apply(Log).Count());
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
        var filter = new MatchLogFilter { Mode = GameMode.OneVsOne, PlayerId = Ksuha.Id, Search = "Kaer" };

        Assert.Single(filter.Apply(Log));
    }

    [Fact]
    public void IsEmpty_IsFalseOnceAnyCriterionIsSet()
    {
        Assert.False(new MatchLogFilter { Search = "x" }.IsEmpty);
        Assert.False(new MatchLogFilter { Mode = GameMode.FreeForAll }.IsEmpty);
        Assert.False(new MatchLogFilter { From = DateTime.Today }.IsEmpty);
    }
}
