namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;
using Unmatched.Models;

public interface IFirstTournamentRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent, Stage stage);
}