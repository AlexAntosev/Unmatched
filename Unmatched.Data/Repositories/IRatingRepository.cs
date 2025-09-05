namespace Unmatched.Data.Repositories;

using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;

public interface IRatingRepository : IRepository<Rating>
{
    Task<Rating?> GetByHeroIdAsync(Guid heroId);

    Task ClearAllRatingsAsync();
}
