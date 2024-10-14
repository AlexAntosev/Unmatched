namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class MapRepository : BaseRepository<Map>, IMapRepository
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
