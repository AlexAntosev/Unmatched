namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public interface ITitleService
{
    Task AddAsync(TitleDto title);

    Task<IEnumerable<TitleDto>> GetAsync();

    Task DeleteAsync(Guid id);
    
    Task MergeAsync(Guid titleId, IEnumerable<Guid> heroesIds);
    Task<IEnumerable<HeroTitleAssignDto>> GetHeroesForTitleAssign(Guid titleId);
}
