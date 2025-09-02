namespace Unmatched.Tests;

using Moq;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;

using Match = Unmatched.Data.Entities.Match;

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