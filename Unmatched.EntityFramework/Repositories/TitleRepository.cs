namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class TitleRepository : BaseRepository<Title>, ITitleRepository
{
    public TitleRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IEnumerable<Title>> GetByHeroId(Guid heroId)
    {
        var entities = await DbContext.Titles.Include(t => t.Heroes).Where(t => t.Heroes.Any(h => h.Id == heroId)).ToListAsync();

        return entities;
    }

    public async Task<Title?> GetByNameAsync(string name)
    {
        var entity = await DbContext.Titles.Include(t => t.Heroes).FirstOrDefaultAsync(t => t.Name.Equals(name));

        return entity;
    }

    protected override Guid GetId(Title model)
    {
        return model.Id;
    }
}
