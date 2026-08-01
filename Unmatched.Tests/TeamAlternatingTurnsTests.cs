namespace Unmatched.Tests;

using Unmatched.UI.BlazorServer.Shared.TurnOrder;

/// <summary>
/// The 2v2 turn-numbering rule shared by MatchSheet's drag-confined-to-team renumber/shuffle and
/// the draft session's turn-order roll (design/IMPLEMENTATION-PROMPT.md §5.4): team 1 gets
/// 1, 3, 5…, team 2 gets 2, 4, 6…, each team's own order independent of the other's.
/// </summary>
public class TeamAlternatingTurnsTests
{
    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 3)]
    [InlineData(1, 2, 5)]
    [InlineData(2, 0, 2)]
    [InlineData(2, 1, 4)]
    [InlineData(2, 2, 6)]
    public void TurnFor_TeamAndIndex_AlternatesBetweenTeams(int team, int indexWithinTeam, int expected)
    {
        Assert.Equal(expected, TeamAlternatingTurns.TurnFor(team, indexWithinTeam));
    }

    [Fact]
    public void TurnFor_TeamOneAndTeamTwo_NeverProduceTheSameNumber()
    {
        var teamOneNumbers = Enumerable.Range(0, 4).Select(i => TeamAlternatingTurns.TurnFor(1, i)).ToHashSet();
        var teamTwoNumbers = Enumerable.Range(0, 4).Select(i => TeamAlternatingTurns.TurnFor(2, i)).ToHashSet();

        Assert.Empty(teamOneNumbers.Intersect(teamTwoNumbers));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    public void TurnFor_AnyTeamValueOtherThanTwo_TreatedAsTeamOne(int nonTeamTwoValue)
    {
        Assert.Equal(TeamAlternatingTurns.TurnFor(1, 0), TeamAlternatingTurns.TurnFor(nonTeamTwoValue, 0));
    }
}
