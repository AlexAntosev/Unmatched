using Unmatched.Common.EntityFramework;

namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.EntityFramework.Context;
using Unmatched.CatalogService.EntityFramework.Entities;

public class MinionRepository : BaseRepository<Minion, UnmatchedDbContext>, IMinionRepository
{
    public MinionRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override Guid GetId(Minion model)
    {
        return model.Id;
    }
}
