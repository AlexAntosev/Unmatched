namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;
using Unmatched.Models;

public class UnrankedRatingCalculator : IUnrankedRatingCalculator
{
    public async Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent)
    {
        var winnerFighter = fighter.IsWinner
            ? fighter
            : opponent;
        var looserFighter = fighter.IsWinner
            ? opponent
            : fighter;
        
        var fighterMatchPoints = new HeroMatchPoints
            {
                HeroId = winnerFighter.HeroId,
                Points = 250
            };

        var opponentMatchPoints = new HeroMatchPoints
            {
                HeroId = looserFighter.HeroId,
                Points = -150
            };

        return 
            new[]
                {
                    fighterMatchPoints,
                    opponentMatchPoints
                };
    }
}
