namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;

public interface IMatchHandlerFactory
{
    public IMatchHandler Create(Match match);
}
