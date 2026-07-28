namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

// Placeholder: FreeForAll matches don't affect Hero rating yet. Once real scoring logic is
// designed, dropping it in here and calling RatingService.RecalculateAsync() will retroactively
// (re)score all historical FreeForAll matches.
public class FreeForAllRatingCalculator : IFreeForAllRatingCalculator
{
    public Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match)
        => Task.FromResult(new Dictionary<Guid, int>());
}
