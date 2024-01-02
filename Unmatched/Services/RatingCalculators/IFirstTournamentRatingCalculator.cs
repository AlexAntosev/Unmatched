namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;
using Unmatched.Enums;
using Unmatched.Models;

public interface IFirstTournamentRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent, Stage stage);
}