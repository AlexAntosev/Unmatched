namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;
using Unmatched.CatalogService.EntityFramework.Context;

public class SidekickRepository(UnmatchedDbContext dbContext) : BaseRepository<Sidekick, UnmatchedDbContext>(dbContext), ISidekickRepository
{
    protected override Guid GetId(Sidekick model)
    {
        return model.Id;
    }

    public IEnumerable<Sidekick> GetByHero(Guid heroId)
    {
        return DbContext.Sidekicks.Where(x => x.HeroId == heroId).ToList();
    }
}
