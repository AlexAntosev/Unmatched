using Unmatched.Common.EntityFramework;

namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.EntityFramework.Context;
using Unmatched.CatalogService.EntityFramework.Entities;

public class MapRepository : BaseRepository<Map, UnmatchedDbContext>, IMapRepository
{
    public MapRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    public Guid GetIdByName(string name)
    {
        return DbContext.Maps.First(x => x.Name.Equals(name)).Id;
    }

    protected override Guid GetId(Map model)
    {
        return model.Id;
    }
}
