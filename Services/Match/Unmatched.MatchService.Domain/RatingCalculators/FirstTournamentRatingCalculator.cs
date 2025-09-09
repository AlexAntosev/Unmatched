namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;
using Unmatched.MatchService.Domain.Repositories;

public class FirstTournamentRatingCalculator : IFirstTournamentRatingCalculator
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ICatalogHeroCache _catalogHeroCache;

    public FirstTournamentRatingCalculator(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache)
    {
        _unitOfWork = unitOfWork;
        _catalogHeroCache = catalogHeroCache;
    }

    public async Task<Dictionary<Guid, int>> CalculateAsync(FighterEntity fighter, FighterEntity opponent, Stage stage)
    {
        var winner = fighter.IsWinner
            ? fighter
            : opponent;
        var looser = fighter.IsWinner
            ? opponent
            : fighter;

        var winnerHeroMaxHp = (await _catalogHeroCache.GetAsync()).First(h => h.Id == winner.HeroId).Hp;
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
