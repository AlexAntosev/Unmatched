using Unmatched.Entities;

namespace Unmatched.Services;

public interface IRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent);
}

public struct HeroMatchPoints
{
    public Guid HeroId { get; set; }
    public int Points { get; set; }
}