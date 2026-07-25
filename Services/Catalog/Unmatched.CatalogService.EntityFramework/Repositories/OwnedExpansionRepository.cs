namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;
using Unmatched.CatalogService.EntityFramework.Context;

public class OwnedExpansionRepository(UnmatchedDbContext dbContext) : BaseRepository<OwnedExpansion, UnmatchedDbContext>(dbContext), IOwnedExpansionRepository
{
    protected override Guid GetId(OwnedExpansion model)
    {
        return model.Id;
    }
}
