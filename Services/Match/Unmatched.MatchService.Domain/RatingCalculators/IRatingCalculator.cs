namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

public interface IRatingCalculator
{
    /// <returns>Rating point change per hero id. A hero absent from the result is treated as
    /// unchanged (0), not "not found" - <see cref="NoRatingCalculator"/> returns an empty result for
    /// exactly this reason.</returns>
    Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match);
}
