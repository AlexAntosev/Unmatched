namespace Unmatched.Services.RatingCalculators;

using Unmatched.Data.Entities;

public interface IUnrankedRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent);
}
