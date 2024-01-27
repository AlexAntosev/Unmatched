namespace Unmatched.Data.Repositories;

using System;

public interface IUnitOfWork : IDisposable
{
    IFavoritesRepository Favorites { get; }

    IFighterRepository Fighters { get; }
    
    IHeroRepository Heroes { get; }
    
    IMapRepository Maps { get; }
    
    IMatchRepository Matches { get; }
    
    IPlayerRepository Players { get; }
    
    IRatingRepository Ratings { get; }
    
    ISidekickRepository Sidekicks { get; }
    
    ITournamentRepository Tournaments { get; }
    
    ITitleRepository Titles { get; }
    
    Task SaveChangesAsync();
}
