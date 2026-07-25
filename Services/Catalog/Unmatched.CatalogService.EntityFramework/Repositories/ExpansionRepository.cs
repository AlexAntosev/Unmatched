namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;
using Unmatched.CatalogService.EntityFramework.Context;

public class ExpansionRepository(UnmatchedDbContext dbContext) : BaseRepository<Expansion, UnmatchedDbContext>(dbContext), IExpansionRepository
{
    protected override Guid GetId(Expansion model)
    {
        return model.Id;
    }

    public override async Task<IReadOnlyList<Expansion>> GetAsync()
    {
        return await DbContext.Set<Expansion>()
            .AsNoTracking()
            .Include(e => e.Heroes)
            .Include(e => e.Maps)
            .ToListAsync();
    }
}
