namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class MinionRepository : BaseRepository<Minion>, IMinionRepository
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
