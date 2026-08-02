namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class TitleRepository(UnmatchedDbContext dbContext) : BaseRepository<TitleEntity, UnmatchedDbContext>(dbContext), ITitleRepository
{
    public override async Task<IReadOnlyList<TitleEntity>> GetAsync()
    {
        var entities = await DbContext.Titles.Include(t => t.HeroTitles).AsNoTracking().ToListAsync();

        return entities;
    }

    public async Task<IEnumerable<TitleEntity>> GetByHeroId(Guid heroId)
    {
        var entities = await DbContext.Titles.Include(t => t.HeroTitles).Where(t => t.HeroTitles.Any(h => h.HeroesId == heroId)).AsNoTracking().ToListAsync();

        return entities;
    }

    public async Task<TitleEntity?> GetByRuleKeyAsync(string ruleKey)
    {
        var entity = await DbContext.Titles.Include(t => t.HeroTitles).FirstOrDefaultAsync(t => t.RuleKey == ruleKey);

        return entity;
    }

    public override async Task<TitleEntity?> GetByIdAsync(Guid id)
    {
        // tracked (not AsNoTracking): TitleService mutates HeroTitles in place (Assign/Unassign/Merge) and
        // relies on the change tracker to detect additions/removals to this collection on SaveChangesAsync.
        var entity = await DbContext.Titles.Include(t => t.HeroTitles).FirstOrDefaultAsync(t => t.Id == id);

        return entity;
    }

    protected override Guid GetId(TitleEntity model)
    {
        return model.Id;
    }
}