namespace Unmatched.Tests;

using Unmatched.Entities;
using Unmatched.Services.MatchHandlers;

using Match = Unmatched.Entities.Match;

public class BaseMatchHandlerTests
{
    [Fact]
    public async Task HandleAsync_NoFighters_ThrowException()
    {
        var match = new Match();

        var baseHandler = new TestMatchHandler();
        await Assert.ThrowsAsync<ArgumentException>(() => baseHandler.HandleAsync(match));
    }

    [Fact]
    public async Task  HandleAsync_NotEnoughFighters_ThrowException()
    {
        var match = new Match
            {
                Fighters = new List<Fighter>()
                    {
                        new()
                    }
            };
        var baseHandler = new TestMatchHandler();
        await Assert.ThrowsAsync<ArgumentException>(() => baseHandler.HandleAsync(match));
    }
}

public class TestMatchHandler : BaseMatchHandler
{
    protected override async Task InnerHandleAsync(Match match)
    {
    }
}