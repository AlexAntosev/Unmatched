namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Models;

public interface IRatingService
{
    Task<Rating> GetByHeroAsync(Guid heroId);

    Task<List<RatingChange>> GetRatingChangesAsync(Guid heroId);

    public Task RecalculateAsync();

    Task<IEnumerable<Rating>> GetAllAsync();
}
