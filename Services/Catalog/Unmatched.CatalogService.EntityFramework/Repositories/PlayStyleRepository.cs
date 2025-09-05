namespace Unmatched.CatalogService.EntityFramework.Repositories;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.CatalogService.EntityFramework.Context;
using Unmatched.CatalogService.EntityFramework.Entities;
using Unmatched.Common.EntityFramework;

public class PlayStyleRepository : BaseRepository<PlayStyle, UnmatchedDbContext>, IPlayStyleRepository
{
    public PlayStyleRepository(UnmatchedDbContext dbContext) : base(dbContext)
    {
    }
    
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
