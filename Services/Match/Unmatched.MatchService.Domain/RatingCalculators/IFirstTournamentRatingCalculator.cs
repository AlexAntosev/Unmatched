namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

public interface IFirstTournamentRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(FighterEntity fighter, FighterEntity opponent, Stage stage);
}