namespace Unmatched.Services;

using Unmatched.DataInitialization;
using Unmatched.Entities;

public interface IFirstTournamentRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent, MatchLevel matchLevel);
}