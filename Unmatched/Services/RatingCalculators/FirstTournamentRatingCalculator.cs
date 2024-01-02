namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;
using Unmatched.Enums;
using Unmatched.Models;
using Unmatched.Repositories;

public class FirstTournamentRatingCalculator : IFirstTournamentRatingCalculator
{
    private readonly IUnitOfWork _unitOfWork;

    public FirstTournamentRatingCalculator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent, Stage stage)
    {
        var winner = fighter.IsWinner
            ? fighter
            : opponent;
        var looser = fighter.IsWinner
            ? opponent
            : fighter;

        var winnerHeroMaxHp = (await _unitOfWork.Heroes.GetByIdAsync(winner.HeroId)).Hp;
        
        var coefficient = stage switch
            {
                Stage.Group => 2,
                Stage.QuarterFinals => 8,
                Stage.SemiFinals => 8,
                Stage.ThirdPlaceDecider => 8,
                Stage.GrandFinals => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
            };

        var winnerPoints = Convert.ToInt32(Math.Round(coefficient * (80 + 40 * ((double)winner.HpLeft / winnerHeroMaxHp)), 0));

        return new Dictionary<Guid, int>
            {
                {
                    winner.HeroId, winnerPoints
                },
                {
                    looser.HeroId, 0
                },
            };
    }
}
