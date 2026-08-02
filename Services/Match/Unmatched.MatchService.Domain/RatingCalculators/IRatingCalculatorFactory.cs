namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Enums;

public interface IRatingCalculatorFactory
{
    IRatingCalculator Create(GameMode gameMode);
}
