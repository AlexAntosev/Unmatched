namespace Unmatched.Services.RatingCalculators;

using Unmatched.Data.Entities;
using Unmatched.Data.Enums;

public interface IFirstTournamentRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent, Stage stage);
}