namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class VillainRepository : BaseRepository<Villain>, IVillainRepository
{
    public VillainRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override Guid GetId(Villain model)
    {
        return model.Id;
    }
}
