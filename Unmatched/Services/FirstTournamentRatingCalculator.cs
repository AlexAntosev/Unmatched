namespace Unmatched.Services;

using Unmatched.DataInitialization;
using Unmatched.Entities;
using Unmatched.Repositories;

public class FirstTournamentRatingCalculator : IFirstTournamentRatingCalculator
{
    private readonly IHeroRepository _heroRepository;

    public FirstTournamentRatingCalculator(IHeroRepository heroRepository)
    {
        _heroRepository = heroRepository;
    }

    public async Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent, MatchLevel matchLevel)
    {
        var winnerHeroId = fighter.IsWinner
            ? fighter.HeroId
            : opponent.HeroId;
        
        var winnerHp = fighter.IsWinner
            ? fighter.HpLeft
            : opponent.HpLeft;
        
        var looserHeroId = fighter.IsWinner
            ? opponent.HeroId
            : fighter.HeroId;

        var winnerHeroMaxHp = (await _heroRepository.GetByIdAsync(winnerHeroId)).Hp;
        
        var coeficient = matchLevel switch
            {
                MatchLevel.Group => 2,
                MatchLevel.QuarterFinals => 8,
                MatchLevel.SemiFinals => 8,
                MatchLevel.ThirdPlaceFinals => 8,
                MatchLevel.Finals => 4,
            };

        var winnerPoints = Convert.ToInt32(Math.Round(coeficient * (80 + 40 * ((double)winnerHp / winnerHeroMaxHp)), 0));

        return new List<HeroMatchPoints>()
            {
                new()
                    {
                        HeroId = looserHeroId,
                        Points = 0
                    },
                new()
                    {
                        HeroId = winnerHeroId,
                        Points = winnerPoints
                    },
            };
    }
}
