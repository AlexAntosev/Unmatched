namespace Unmatched.EntityFramework.Repositories;

using System;
using Microsoft.EntityFrameworkCore;
using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

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
