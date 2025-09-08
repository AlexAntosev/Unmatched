namespace Unmatched.PlayerService.EntityFramework.Repositories;

using Unmatched.PlayerService.Domain.Repositories;
using Unmatched.PlayerService.EntityFramework.Context;

public class UnitOfWork : IUnitOfWork
{
    private readonly UnmatchedDbContext _context;

    public IFavoritesRepository Favorites { get; }


    public IPlayerRepository Players { get; }

    
    public UnitOfWork(UnmatchedDbContext context)
    {
        _context = context;
        Favorites = new FavoritesRepository(context);
        Players = new PlayerRepository(context);
    }

    public Task SaveChangesAsync() 
        => _context.SaveChangesAsync();

    public void Dispose() 
        => _context.Dispose();
}
