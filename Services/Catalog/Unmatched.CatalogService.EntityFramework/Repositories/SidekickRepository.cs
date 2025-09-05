namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.EntityFramework.Context;
using Unmatched.CatalogService.EntityFramework.Entities;
using Unmatched.Common.EntityFramework;

public class SidekickRepository : BaseRepository<Sidekick, UnmatchedDbContext>, ISidekickRepository
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
