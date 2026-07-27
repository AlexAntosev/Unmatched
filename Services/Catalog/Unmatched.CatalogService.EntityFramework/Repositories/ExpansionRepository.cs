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
        // The collection screen shows everything that ships in a box, so all four content
        // collections are loaded rather than just heroes and maps.
        return await DbContext.Set<Expansion>()
            .AsNoTracking()
            .Include(e => e.Heroes)
            .Include(e => e.Maps)
            .Include(e => e.Villains)
            .Include(e => e.Minions)
            .ToListAsync();
    }
}
