namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;
using Unmatched.CatalogService.EntityFramework.Context;

public class MapRepository(UnmatchedDbContext dbContext) : BaseRepository<Map, UnmatchedDbContext>(dbContext), IMapRepository
{
    public Guid GetIdByName(string name)
    {
        return DbContext.Maps.First(x => x.Name.Equals(name)).Id;
    }

    protected override Guid GetId(Map model)
    {
        return model.Id;
    }
}
