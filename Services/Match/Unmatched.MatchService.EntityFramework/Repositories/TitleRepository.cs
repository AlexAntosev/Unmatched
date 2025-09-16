namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class TitleRepository(UnmatchedDbContext dbContext) : BaseRepository<TitleEntity, UnmatchedDbContext>(dbContext), ITitleRepository
{
    public async Task<IEnumerable<TitleEntity>> GetByHeroId(Guid heroId)
    {
        var entities = await DbContext.Titles.Include(t => t.HeroTitles).Where(t => t.HeroTitles.Any(h => h.HeroesId == heroId)).AsNoTracking().ToListAsync();

        return entities;
    }

    public async Task<TitleEntity?> GetByNameAsync(string name)
    {
        var entity = await DbContext.Titles.Include(t => t.HeroTitles).AsNoTracking().FirstOrDefaultAsync(t => t.Name.Equals(name));

        return entity;
    }

    protected override Guid GetId(TitleEntity model)
    {
        return model.Id;
    }
}