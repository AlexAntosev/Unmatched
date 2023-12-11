namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;

public interface IMatchHandler
{
    Task HandleAsync(Match match);
}
