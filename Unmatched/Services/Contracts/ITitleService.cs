namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface ITitleService
{
    Task AddAsync(TitleDto title);

    Task<IEnumerable<TitleDto>> GetAsync();

    Task DeleteAsync(Guid id);

    Task AssignAsync(Guid titleId, Guid heroId);
    
    Task UnassignAsync(Guid titleId, Guid heroId);
    
    Task MergeAsync(Guid titleId, IEnumerable<Guid> heroesIds);
    Task<IEnumerable<HeroTitleAssignDto>> GetHeroesForTitleAssign(Guid titleId);
}
