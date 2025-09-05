namespace Unmatched.Data.Repositories;

using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;

public interface ITitleRepository : IRepository<Title>
{
    Task<IEnumerable<Title>> GetByHeroId(Guid heroId);
    
    Task<Title?> GetByNameAsync(string name);
}
