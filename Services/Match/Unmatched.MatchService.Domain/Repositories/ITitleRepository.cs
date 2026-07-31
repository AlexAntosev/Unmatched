namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface ITitleRepository : IRepository<TitleEntity>
{
    Task<IEnumerable<TitleEntity>> GetByHeroId(Guid heroId);

    /// <summary>Tracked (not AsNoTracking): TitleEvaluator mutates HeroTitles in place and relies on the
    /// change tracker to detect additions/removals on SaveChangesAsync - same reasoning as GetByIdAsync.</summary>
    Task<TitleEntity?> GetByRuleKeyAsync(string ruleKey);
}
