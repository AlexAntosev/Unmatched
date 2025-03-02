namespace Unmatched.EntityFramework.Repositories;

using System;
using Microsoft.EntityFrameworkCore;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class FavoritesRepository : BaseRepository<Favorite>, IFavoritesRepository
{
    public FavoritesRepository(UnmatchedDbContext dbContext) : base(dbContext)
    {
    }
    public async Task<List<Favorite>> GetByPlayerIdAsync(Guid playerId)
    {
        return await DbContext.Set<Favorite>().Include(f => f.Player).Include(f => f.Hero).Where(f => f.PlayerId == playerId).AsNoTracking().ToListAsync();
    }

    protected override Guid GetId(Favorite model)
    {
        return model.Id;
    }
}
