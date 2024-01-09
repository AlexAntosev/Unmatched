namespace Unmatched.Services.MatchHandlers;

using Unmatched.Data.Entities;

public interface IMatchHandler : IDisposable
{
    Task HandleAsync(Match match);
}
