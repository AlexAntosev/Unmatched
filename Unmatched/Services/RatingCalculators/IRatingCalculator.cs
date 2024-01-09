namespace Unmatched.Services.RatingCalculators;

using Unmatched.Data.Entities;

public interface IRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent);
}