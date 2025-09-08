namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;
using Unmatched.CatalogService.EntityFramework.Context;

public class MinionRepository(UnmatchedDbContext dbContext) : BaseRepository<Minion, UnmatchedDbContext>(dbContext), IMinionRepository
{
    protected override Guid GetId(Minion model)
    {
        return model.Id;
    }
}
