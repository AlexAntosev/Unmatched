namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Models;

public interface ITitleService
{
    Task AddAsync(Title title);

    Task<IEnumerable<Title>> GetAsync();

    Task DeleteAsync(Guid id);

    Task AssignAsync(Guid titleId, Guid heroId);
    
    Task UnassignAsync(Guid titleId, Guid heroId);
    
    Task MergeAsync(Guid titleId, IEnumerable<Guid> heroesIds);

    Task<IEnumerable<HeroTitleAssign>> GetHeroesForTitleAssign(Guid titleId);
}
