namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;

public interface IMatchHandler : IDisposable
{
    Task HandleAsync(MatchEntity match);
}
