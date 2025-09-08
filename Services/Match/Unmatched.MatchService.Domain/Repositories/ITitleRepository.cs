namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface ITitleRepository : IRepository<Title>
{
    Task<IEnumerable<Title>> GetByHeroId(Guid heroId);
    
    Task<Title?> GetByNameAsync(string name);
}
