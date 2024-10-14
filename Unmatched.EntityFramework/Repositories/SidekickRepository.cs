namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class SidekickRepository : BaseRepository<Sidekick>, ISidekickRepository
{
    public SidekickRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override Guid GetId(Sidekick model)
    {
        return model.Id;
    }

    public IEnumerable<Sidekick> GetByHero(Guid heroId)
    {
        return DbContext.Sidekicks.Where(x => x.HeroId == heroId).ToList();
    }
}
