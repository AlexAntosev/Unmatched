namespace Unmatched.PlayerService.Domain.Repositories;

using System;

public interface IUnitOfWork : IDisposable
{
    IFavoritesRepository Favorites { get; }
    
    IPlayerRepository Players { get; }
    
    Task SaveChangesAsync();
}
