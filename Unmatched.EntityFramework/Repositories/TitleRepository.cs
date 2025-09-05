namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class TitleRepository(UnmatchedDbContext dbContext) : BaseRepository<Title, UnmatchedDbContext>(dbContext), ITitleRepository
{
    public async Task<IEnumerable<Title>> GetByHeroId(Guid heroId)
    {
       // var entities = await DbContext.Titles.Include(t => t.Heroes).Where(t => t.Heroes.Any(h => h.Id == heroId)).AsNoTracking().ToListAsync();
        var entities = await DbContext.Titles.Include(t => t.HeroTitles).Where(t => t.HeroTitles.Any(h => h.HeroesId == heroId)).AsNoTracking().ToListAsync();

        return entities;
    }

    public async Task<Title?> GetByNameAsync(string name)
    {
       // var entity = await DbContext.Titles.Include(t => t.Heroes).AsNoTracking().FirstOrDefaultAsync(t => t.Name.Equals(name));
        var entity = await DbContext.Titles.Include(t => t.HeroTitles).AsNoTracking().FirstOrDefaultAsync(t => t.Name.Equals(name));

        return entity;
    }

    protected override Guid GetId(Title model)
    {
        return model.Id;
    }
}
