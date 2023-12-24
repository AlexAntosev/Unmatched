namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;
using Unmatched.Models;

public interface IRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent);
}