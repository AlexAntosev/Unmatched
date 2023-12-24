namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;
using Unmatched.Models;

public interface IRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent);
}