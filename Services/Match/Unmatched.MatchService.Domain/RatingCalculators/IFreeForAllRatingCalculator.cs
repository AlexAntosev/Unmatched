namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

public interface IFreeForAllRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match);
}
