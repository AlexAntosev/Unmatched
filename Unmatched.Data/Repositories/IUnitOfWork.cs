namespace Unmatched.Data.Repositories;

using System;

public interface IUnitOfWork : IDisposable
{
    IFavoritesRepository Favorites { get; }

    IFighterRepository Fighters { get; }
    
    IMapRepository Maps { get; }
    
    IMatchRepository Matches { get; }
    
    IMinionRepository Minions { get; }
    
    IPlayerRepository Players { get; }
    
    IRatingRepository Ratings { get; }
    
    ITournamentRepository Tournaments { get; }
    
    ITitleRepository Titles { get; }
    
    IVillainRepository Villains { get; }
    
    Task SaveChangesAsync();
}
