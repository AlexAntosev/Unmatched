namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

// Placeholder: Cooperative matches don't affect Hero rating yet (there's no opposing hero to
// compare against a Villain). Once real scoring logic is designed, dropping it in here and calling
// RatingService.RecalculateAsync() will retroactively (re)score all historical Cooperative matches.
public class CooperativeRatingCalculator : ICooperativeRatingCalculator
{
    public Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match)
        => Task.FromResult(new Dictionary<Guid, int>());
}
