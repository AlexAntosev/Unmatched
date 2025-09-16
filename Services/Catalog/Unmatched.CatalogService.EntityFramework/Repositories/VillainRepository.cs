namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;
using Unmatched.CatalogService.EntityFramework.Context;

public class VillainRepository(UnmatchedDbContext dbContext) : BaseRepository<Villain, UnmatchedDbContext>(dbContext), IVillainRepository
{
    protected override Guid GetId(Villain model)
    {
        return model.Id;
    }
}
