namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class HeroRepository : BaseRepository<Hero>, IHeroRepository
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
