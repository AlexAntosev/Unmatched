namespace Unmatched.CatalogService.EntityFramework.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Unmatched.CatalogService.EntityFramework.Context;
using Unmatched.CatalogService.EntityFramework.Entities;
using Unmatched.Common.EntityFramework;

public class HeroRepository : BaseRepository<Hero, UnmatchedDbContext>, IHeroRepository
{
    public HeroRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    public override IQueryable<Hero> Query(bool noTrack = false)
    {
        return noTrack ? DbContext.Heroes.Include(x => x.Sidekicks).AsNoTracking() : DbContext.Heroes.Include(x => x.Sidekicks);
    }

    public Guid GetIdByName(string name)
    {
        return DbContext.Heroes.First(x => x.Name.Equals(name)).Id;
    }

    protected override Guid GetId(Hero model)
    {
        return model.Id;
    }

    public override Task<List<Hero>> GetAsync()
    {
        return DbContext.Set<Hero>().Include(x => x.Sidekicks).AsNoTracking().ToListAsync();
    }
}
