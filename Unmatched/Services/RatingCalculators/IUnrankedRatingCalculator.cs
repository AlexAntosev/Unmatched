namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;

public interface IUnrankedRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent);
}
