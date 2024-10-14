namespace Unmatched.EntityFramework.Repositories;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class PlayStyleRepository : BaseRepository<PlayStyle>, IPlayStyleRepository
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
