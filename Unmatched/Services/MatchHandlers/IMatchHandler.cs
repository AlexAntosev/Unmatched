namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;

public interface IMatchHandler : IDisposable
{
    Task HandleAsync(Match match);
}
