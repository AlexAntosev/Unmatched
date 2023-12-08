namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface IRatingRepository : IRepository<Rating>
{
    Task<Rating?> GetByHeroIdAsync(Guid heroId);

    Task ClearAllRatingsAsync();
}
