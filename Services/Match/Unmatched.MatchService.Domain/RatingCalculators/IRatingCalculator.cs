namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

public interface IRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent);
}