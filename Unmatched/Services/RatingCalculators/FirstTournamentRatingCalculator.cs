namespace Unmatched.Services.RatingCalculators;

using Unmatched.Data.Entities;
using Unmatched.Data.Enums;
using Unmatched.Data.Repositories;
using Unmatched.Extensions;

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
        var winnerPoints = Convert.ToInt32(Math.Round(stage.GetCoefficient() * (80 + 40 * ((double)winner.HpLeft / winnerHeroMaxHp)), 0));

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
