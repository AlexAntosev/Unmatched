namespace Unmatched.Services;

using Unmatched.DataInitialization;

public interface IFirstTournamentRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(FirstTournamentMatchInfo matchInfo);
}