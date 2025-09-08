namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface IRatingRepository : IRepository<Rating>
{
    Task<Rating?> GetByHeroIdAsync(Guid heroId);

    Task ClearAllRatingsAsync();
}
