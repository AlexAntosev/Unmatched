namespace Unmatched.Services.MatchHandlers;

using Unmatched.Data.Entities;

public interface IMatchHandlerFactory
{
    public IMatchHandler Create(Match match);
}
