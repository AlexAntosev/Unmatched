namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

using Match = Unmatched.MatchService.Domain.Entities.Match;

public class BaseMatchHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    
    [Fact]
    public async Task HandleAsync_NoFighters_ThrowException()
    {
        var match = new Match();

        var baseHandler = new TestMatchHandler(_unitOfWork.Object);
        
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
        var baseHandler = new TestMatchHandler(_unitOfWork.Object);
        await Assert.ThrowsAsync<ArgumentException>(() => baseHandler.HandleAsync(match));
    }
}