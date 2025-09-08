namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

public class UnrankedRatingCalculator : IUnrankedRatingCalculator
{
    public async Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent)
    {
        var winner = fighter.IsWinner
            ? fighter
            : opponent;
        var looser = fighter.IsWinner
            ? opponent
            : fighter;

        return new Dictionary<Guid, int>
            {
                {
                    winner.HeroId, 250
                },
                {
                    looser.HeroId, -150
                },
            };
    }
}
