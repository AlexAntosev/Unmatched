namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;
using Unmatched.Models;
using Unmatched.Repositories;

public class FirstTournamentRatingCalculator : IFirstTournamentRatingCalculator
{
    private readonly IUnitOfWork _unitOfWork;

    public FirstTournamentRatingCalculator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        var winnerHeroMaxHp = (await _unitOfWork.Heroes.GetByIdAsync(winnerHeroId)).Hp;
        
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
