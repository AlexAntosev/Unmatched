using Unmatched.Common.EntityFramework;

namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.EntityFramework.Context;
using Unmatched.CatalogService.EntityFramework.Entities;

public class VillainRepository(UnmatchedDbContext dbContext) : BaseRepository<Villain, UnmatchedDbContext>(dbContext), IVillainRepository
{
    protected override Guid GetId(Villain model)
    {
        return model.Id;
    }
}
