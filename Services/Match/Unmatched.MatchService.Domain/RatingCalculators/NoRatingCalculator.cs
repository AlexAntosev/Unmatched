namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

/// <summary>Used for Unranked matches and, permanently, for Cooperative matches: villains and
/// minions aren't on the hero rating ladder, so paying heroes out of a villain's rating (or vice
/// versa) would leak points into and out of a ladder that's supposed to be closed. Co-op still gets
/// a win/loss record - see VillainStats/MinionStats - just not an Elo delta.</summary>
public class NoRatingCalculator : IRatingCalculator
{
    public Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match)
        => Task.FromResult(new Dictionary<Guid, int>());
}
