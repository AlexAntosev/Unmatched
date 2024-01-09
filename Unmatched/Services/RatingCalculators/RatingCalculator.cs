namespace Unmatched.Services.RatingCalculators;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Models;

public class RatingCalculator : IRatingCalculator
{
    private readonly IUnitOfWork _unitOfWork;

    public RatingCalculator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Dictionary<Guid, int>> CalculateAsync(Fighter fighter, Fighter opponent)
    {
        var winner = fighter.IsWinner
            ? fighter
            : opponent;
        var looser = fighter.IsWinner
            ? opponent
            : fighter;

        var matchContext = new MatchContext(
            await _unitOfWork.Heroes.GetByIdAsync(winner.HeroId),
            await _unitOfWork.Heroes.GetByIdAsync(looser.HeroId),
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

    private int CalculateForLooser(MatchContext matchContext)
    {
        var maxLooserSidekicksHp = matchContext.LooserReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;
        var maxWinnerSidekicksHp = matchContext.WinnerReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;

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

    private int CalculateForWinner(MatchContext matchContext)
    {
        var maxLooserSidekicksHp = matchContext.LooserReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;
        var maxWinnerSidekicksHp = matchContext.WinnerReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;

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
