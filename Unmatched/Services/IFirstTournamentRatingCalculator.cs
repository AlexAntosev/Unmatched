namespace Unmatched.Services;

using Unmatched.DataInitialization;

public interface IFirstTournamentRatingCalculator
{
    Task<IEnumerable<HeroMatchPoints>> CalculateAsync(FirstTournamentMatchInfo matchInfo);
}

public class FirstTournamentRatingCalculator : IFirstTournamentRatingCalculator
{
    public Task<IEnumerable<HeroMatchPoints>> CalculateAsync(FirstTournamentMatchInfo matchInfo)
    {
        var basePoints = matchInfo.MatchLevel switch
            {
                MatchLevel.Group => 40,
                MatchLevel.QuarterFinals => 40,
                MatchLevel.SemiFinals => 40,
                MatchLevel.ThirdPlaceFinals => 40,
                MatchLevel.Finals => 40,
            };
    }
}
