namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class RatingCalculator : IRatingCalculator
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ICatalogHeroCache _catalogHeroCache;

    public RatingCalculator(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache)
    {
        _unitOfWork = unitOfWork;
        _catalogHeroCache = catalogHeroCache;
    }

    public async Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent)
    {
        var winner = fighter.IsWinner
            ? fighter
            : opponent;
        var looser = fighter.IsWinner
            ? opponent
            : fighter;

        var matchContext = new MatchContextDto(
            await _catalogHeroCache.GetAsync(winner.HeroId),
            await _catalogHeroCache.GetAsync(looser.HeroId),
            winner,
            looser,
            (await _unitOfWork.Ratings.GetByHeroIdAsync(winner.HeroId))?.Points ?? 0,
            (await _unitOfWork.Ratings.GetByHeroIdAsync(looser.HeroId))?.Points ?? 0);

        return new Dictionary<Guid, int>
            {
                {
                    winner.HeroId, CalculateForWinner(matchContext)
                },
                {
                    looser.HeroId, CalculateForLooser(matchContext)
                },
            };
    }

    private int CalculateForLooser(MatchContextDto matchContext)
    {
        var maxLooserSidekicksHp = 0;// matchContext.LooserReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;
        var maxWinnerSidekicksHp = 0;// matchContext.WinnerReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;

        var forSidekickHp = maxLooserSidekicksHp == 0
            ? 50
            : Convert.ToInt32(50.0 * (1.0 - Math.Round((double)matchContext.LooserFighter.SidekickHpLeft / maxLooserSidekicksHp, 1, MidpointRounding.ToPositiveInfinity)));
        var forWinnerHpLeft = Convert.ToInt32(
            150
          * Math.Round(
                (double)(matchContext.WinnerFighter.HpLeft + matchContext.WinnerFighter.SidekickHpLeft) / (matchContext.WinnerReferenceHero.Hp + maxWinnerSidekicksHp),
                1,
                MidpointRounding.ToPositiveInfinity));
        var result = -(100 + forSidekickHp + forWinnerHpLeft);
        return result;
    }

    private int CalculateForWinner(MatchContextDto matchContext)
    {
        var maxLooserSidekicksHp = 0;// matchContext.LooserReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;
        var maxWinnerSidekicksHp = 0;// matchContext.WinnerReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;

        var forLooserSidekick = maxLooserSidekicksHp == 0
            ? 40
            : (int)Math.Round(40.0 * (1.0 - Math.Round((double)matchContext.LooserFighter.SidekickHpLeft / maxLooserSidekicksHp, 1, MidpointRounding.ToPositiveInfinity)), 0);

        var forCardsLeft = Convert.ToInt32(
            80 * Math.Round(matchContext.WinnerFighter.CardsLeft.Value / (0.8 * matchContext.WinnerReferenceHero.DeckSize), 1, MidpointRounding.ToPositiveInfinity));
        var forHandicap = Convert.ToInt32(
            matchContext.LooserPointsBeforeMatch > matchContext.WinnerPointsBeforeMatch
                ? 100 * ((matchContext.LooserPointsBeforeMatch - matchContext.WinnerPointsBeforeMatch) / 500)
                : 0);
        var forHpLeft = Convert.ToInt32(
            80
          * Math.Round(
                (double)(matchContext.WinnerFighter.HpLeft + matchContext.WinnerFighter.SidekickHpLeft) / (matchContext.WinnerReferenceHero.Hp + maxWinnerSidekicksHp),
                1,
                MidpointRounding.ToPositiveInfinity));
        var result = 200 + forLooserSidekick + forCardsLeft + forHandicap + forHpLeft;
        return result;
    }
}
