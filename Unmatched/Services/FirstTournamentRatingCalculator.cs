namespace Unmatched.Services;

using Unmatched.Entities;
using Unmatched.Repositories;

public class FirstTournamentRatingCalculator : IFirstTournamentRatingCalculator
{
    private readonly IHeroRepository _heroRepository;

    public FirstTournamentRatingCalculator(IHeroRepository heroRepository)
    {
        _heroRepository = heroRepository;
    }

    public async Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent, Stage stage)
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
        
        var coefficient = stage switch
            {
                Stage.Group => 2,
                Stage.QuarterFinals => 8,
                Stage.SemiFinals => 8,
                Stage.ThirdPlaceFinals => 8,
                Stage.Finals => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
            };

        var winnerPoints = Convert.ToInt32(Math.Round(coefficient * (80 + 40 * ((double)winnerHp / winnerHeroMaxHp)), 0));

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
