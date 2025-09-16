namespace Unmatched.PlayerService.EntityFramework.Repositories;

using Unmatched.PlayerService.Domain.Repositories;
using Unmatched.PlayerService.EntityFramework.Context;

public class UnitOfWork(UnmatchedDbContext context) : IUnitOfWork
{
    public IFavoritesRepository Favorites { get; } = new FavoritesRepository(context);

    public IPlayerRepository Players { get; } = new PlayerRepository(context);

    public Task SaveChangesAsync() 
        => context.SaveChangesAsync();

    public void Dispose() 
        => context.Dispose();
}
