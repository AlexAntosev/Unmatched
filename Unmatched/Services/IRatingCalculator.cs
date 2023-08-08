using Unmatched.Entities;

namespace Unmatched.Services;

public interface IRatingCalculator
{
    Task CalculateAsync(Fighter fighter, Fighter opponent, Tournament tournament);
}