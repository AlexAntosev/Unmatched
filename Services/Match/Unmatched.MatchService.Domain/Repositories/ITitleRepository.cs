namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface ITitleRepository : IRepository<TitleEntity>
{
    Task<IEnumerable<TitleEntity>> GetByHeroId(Guid heroId);
    
    Task<TitleEntity?> GetByNameAsync(string name);
}
