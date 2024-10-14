namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class HeroRepository : BaseRepository<Hero>, IHeroRepository
{
    public HeroRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    public override IQueryable<Hero> Query()
    {
        return DbContext.Heroes.Include(x => x.Sidekicks);
    }

    public Guid GetIdByName(string name)
    {
        return DbContext.Heroes.First(x => x.Name.Equals(name)).Id;
    }

    protected override Guid GetId(Hero model)
    {
        return model.Id;
    }
}
