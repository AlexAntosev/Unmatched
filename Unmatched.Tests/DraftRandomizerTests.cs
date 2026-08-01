namespace Unmatched.Tests;

using Unmatched.Dtos;
using Unmatched.Enums;
using Unmatched.UI.BlazorServer.Pages.Draft;

using static Unmatched.Tests.MatchLogBuilder;

/// <summary>
/// Structural tests for the draft's pool/order generation (design/IMPLEMENTATION-PROMPT.md §5A) -
/// these assert on sizes, permutations and set membership rather than the RNG itself, since the
/// shuffle is genuinely random and any exact-order assertion would be flaky by construction.
/// </summary>
public class DraftRandomizerTests
{
    private static List<MapDto> Maps(int count)
        => Enumerable.Range(0, count).Select(i => new MapDto { Id = Guid.NewGuid(), Name = $"Map {i}" }).ToList();

    private static List<UiHeroDto> Heroes(int count)
        => Enumerable.Range(0, count).Select(i => Hero($"Hero {i}")).ToList();

    private static List<UiPlayerDto> Players(int count)
        => Enumerable.Range(0, count).Select(i => Player($"Player {i}")).ToList();

    [Theory]
    [InlineData(2, 3)]
    [InlineData(3, 4)]
    [InlineData(4, 5)]
    public void BuildMapPool_ReturnsExactlyPlayerCountPlusOneMaps(int playerCount, int expectedPoolSize)
    {
        var pool = DraftRandomizer.BuildMapPool(Maps(10), playerCount);

        Assert.Equal(expectedPoolSize, pool.Count);
        Assert.Equal(pool.Count, pool.Select(m => m.Id).Distinct().Count());
    }

    [Theory]
    [InlineData(2, 6)]
    [InlineData(3, 9)]
    [InlineData(4, 12)]
    public void BuildHeroPool_ReturnsExactlyPlayerCountTimesThreeHeroes(int playerCount, int expectedPoolSize)
    {
        var pool = DraftRandomizer.BuildHeroPool(Heroes(20), playerCount);

        Assert.Equal(expectedPoolSize, pool.Count);
        Assert.Equal(pool.Count, pool.Select(h => h.Id).Distinct().Count());
    }

    [Fact]
    public void RollBanOrder_IsAPermutationOfEveryPlayerExactlyOnce()
    {
        var players = Players(4);

        var order = DraftRandomizer.RollBanOrder(players);

        Assert.Equal(players.Count, order.Count);
        Assert.Equal(players.Select(p => p.Id).ToHashSet(), order.Select(p => p.Id).ToHashSet());
    }

    [Fact]
    public void SnakePickOrder_ForwardThenBackward_GivesEveryPlayerExactlyTwoTurns()
    {
        var banOrder = Players(3);

        var snake = DraftRandomizer.SnakePickOrder(banOrder);

        Assert.Equal(6, snake.Count);
        Assert.Equal(banOrder, snake.Take(3));
        Assert.Equal(banOrder.AsEnumerable().Reverse(), snake.Skip(3));
        foreach (var player in banOrder)
        {
            Assert.Equal(2, snake.Count(p => p.Id == player.Id));
        }
    }

    [Fact]
    public void RollTurnOrder_OneVsOne_AssignsSequentialTurnsWithNoTeam()
    {
        var players = Players(2);

        var order = DraftRandomizer.RollTurnOrder(GameMode.OneVsOne, players, new Dictionary<Guid, int>());

        Assert.Equal([1, 2], order.Select(o => o.Turn).OrderBy(t => t));
        Assert.All(order, o => Assert.Null(o.Team));
        Assert.Equal(players.Select(p => p.Id).ToHashSet(), order.Select(o => o.Player.Id).ToHashSet());
    }

    [Fact]
    public void RollTurnOrder_TeamVsTeam_TeamOneGetsOddNumbersTeamTwoGetsEven()
    {
        var players = Players(4);
        var teams = new Dictionary<Guid, int>
        {
            [players[0].Id] = 1, [players[1].Id] = 1,
            [players[2].Id] = 2, [players[3].Id] = 2
        };

        var order = DraftRandomizer.RollTurnOrder(GameMode.TeamVsTeam, players, teams);

        Assert.Equal(4, order.Count);
        Assert.Equal([1, 3], order.Where(o => o.Team == 1).Select(o => o.Turn).OrderBy(t => t));
        Assert.Equal([2, 4], order.Where(o => o.Team == 2).Select(o => o.Turn).OrderBy(t => t));
        Assert.All(order.Where(o => teams[o.Player.Id] == 1), o => Assert.Equal(1, o.Team));
        Assert.All(order.Where(o => teams[o.Player.Id] == 2), o => Assert.Equal(2, o.Team));
    }

    [Fact]
    public void SplitTeamsRandomly_FourPlayers_SplitsTwoAndTwo()
    {
        var players = Players(4);
        var teams = new Dictionary<Guid, int>();

        DraftRandomizer.SplitTeamsRandomly(players, teams);

        Assert.Equal(4, teams.Count);
        Assert.Equal(2, teams.Values.Count(t => t == 1));
        Assert.Equal(2, teams.Values.Count(t => t == 2));
    }
}
