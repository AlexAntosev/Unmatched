namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface IRatingRepository : IRepository<RatingEntity>
{
    Task<RatingEntity?> GetByHeroIdAsync(Guid heroId);

    Task ClearAllRatingsAsync();
}
