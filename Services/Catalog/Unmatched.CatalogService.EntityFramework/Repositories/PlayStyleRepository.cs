namespace Unmatched.CatalogService.EntityFramework.Repositories;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;
using Unmatched.CatalogService.EntityFramework.Context;

public class PlayStyleRepository(UnmatchedDbContext dbContext) : BaseRepository<PlayStyle, UnmatchedDbContext>(dbContext), IPlayStyleRepository
{
    public async Task<PlayStyle?> GetByHeroIdAsync(Guid heroId)
    {
        var entity = await DbContext.PlayStyles.FirstOrDefaultAsync(p => p.HeroId == heroId);
        
        return entity;
    }

    protected override Guid GetId(PlayStyle model)
    {
        return model.Id;
    }
}
