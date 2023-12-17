namespace Unmatched.Repositories;

using System;

public interface IUnitOfWork : IDisposable
{
    IFighterRepository Fighters { get; }
    
    IHeroRepository Heroes { get; }
    
    IMapRepository Maps { get; }
    
    IMatchRepository Matches { get; }
    
    IMatchStageRepository MatchStages { get; }
    
    IPlayerRepository Players { get; }
    
    IRatingRepository Ratings { get; }
    
    ISidekickRepository Sidekicks { get; }
    
    ITournamentRepository Tournaments { get; }
    
    Task SaveChangesAsync();
}
