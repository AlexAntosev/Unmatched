namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;

public interface IMatchHandler
{
    Task HandleAsync(MatchEntity match);
}
