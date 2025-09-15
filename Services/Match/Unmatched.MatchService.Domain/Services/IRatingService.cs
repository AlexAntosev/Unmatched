namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Models;

public interface IRatingService
{
    public Task RecalculateAsync();

    Task<List<RatingChange>> GetRatingChangesAsync(Guid heroId);
}
