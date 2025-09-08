namespace Unmatched.PlayerService.EntityFramework.Repositories;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.PlayerService.Domain.Entities;
using Unmatched.PlayerService.Domain.Repositories;
using Unmatched.PlayerService.EntityFramework.Context;

public class FavoritesRepository(UnmatchedDbContext dbContext) : BaseRepository<Favorite, UnmatchedDbContext>(dbContext), IFavoritesRepository
{
    public async Task<List<Favorite>> GetByPlayerIdAsync(Guid playerId)
    {
        //return await DbContext.Set<Favorite>().Include(f => f.Player).Include(f => f.Hero).Where(f => f.PlayerId == playerId).AsNoTracking().ToListAsync();
        return await DbContext.Set<Favorite>().Include(f => f.Player).Where(f => f.PlayerId == playerId).AsNoTracking().ToListAsync();
    }

    protected override Guid GetId(Favorite model)
    {
        return model.Id;
    }
}
