namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;

public abstract class BaseMatchHandler : IMatchHandler
{
    protected readonly IUnitOfWork UnitOfWork;

    protected BaseMatchHandler(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    public Task HandleAsync(Match match) 
        => IsNotEnoughFighters(match.Fighters)
        ? throw new ArgumentException("Not enough fighters.", nameof(match))
        : InnerHandleAsync(match);

    protected abstract Task InnerHandleAsync(Match match);

    private static bool IsNotEnoughFighters(ICollection<Fighter>? fighters) 
        => fighters is null || fighters.Count < 2;

    public void Dispose() 
        => UnitOfWork.Dispose();
}

