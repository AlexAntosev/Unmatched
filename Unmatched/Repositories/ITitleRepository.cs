namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface ITitleRepository : IRepository<Title>
{
    Task<IEnumerable<Title>> GetByHeroId(Guid heroId);
    
    Task<Title?> GetByNameAsync(string name);
}
