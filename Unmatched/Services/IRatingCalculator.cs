namespace Unmatched.Services;

using Unmatched.Entities;

public interface IRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent);
}