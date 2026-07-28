namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

public interface ICooperativeRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match);
}
