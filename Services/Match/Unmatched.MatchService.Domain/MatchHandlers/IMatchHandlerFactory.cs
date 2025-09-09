namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;

public interface IMatchHandlerFactory
{
    public IMatchHandler Create(MatchEntity match);
}
