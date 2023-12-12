namespace Unmatched.Services;

using Unmatched.Entities;

public interface IFirstTournamentRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent, Stage stage);
}