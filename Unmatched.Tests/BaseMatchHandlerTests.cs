namespace Unmatched.Tests;

using Unmatched.Entities;
using Unmatched.Services.MatchHandlers;

using Match = Unmatched.Entities.Match;

public class BaseMatchHandlerTests
{
    [Fact]
    public void Handle_NoFighters_ThrowException()
    {
        var match = new Match();

        var baseHandler = new TestMatchHandler();
        Assert.Throws<ArgumentException>(() => baseHandler.Handle(match));
    }

    [Fact]
    public void Handle_NotEnoughFighters_ThrowException()
    {
        var match = new Match
            {
                Fighters = new List<Fighter>()
                    {
                        new()
                    }
            };
        var baseHandler = new TestMatchHandler();
        Assert.Throws<ArgumentException>(() => baseHandler.Handle(match));
    }
}

public class TestMatchHandler : BaseMatchHandler
{
    protected override void InnerHandle(Match match)
    {
    }
}