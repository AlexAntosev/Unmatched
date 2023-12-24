namespace Unmatched.Services.RatingCalculators;

using Unmatched.Entities;
using Unmatched.Models;
using Unmatched.Repositories;

public class RatingCalculator : IRatingCalculator
{
    private readonly IUnitOfWork _unitOfWork;

    public RatingCalculator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent)
    {
        var winnerFighter = fighter.IsWinner
            ? fighter
            : opponent;
        var looserFighter = fighter.IsWinner
            ? opponent
            : fighter;

        var matchContext = new MatchContext(
            await _unitOfWork.Heroes.GetByIdAsync(winnerFighter.HeroId),
            await _unitOfWork.Heroes.GetByIdAsync(looserFighter.HeroId),
            winnerFighter,
            looserFighter,
            (await _unitOfWork.Ratings.GetByHeroIdAsync(winnerFighter.HeroId))?.Points ?? 0,
            (await _unitOfWork.Ratings.GetByHeroIdAsync(looserFighter.HeroId))?.Points ?? 0);

        var fighterMatchPoints = new HeroMatchPoints
            {
                HeroId = winnerFighter.HeroId,
                Points = CalculateForWinner(matchContext)
            };

        var opponentMatchPoints = new HeroMatchPoints
            {
                HeroId = looserFighter.HeroId,
                Points = CalculateForLooser(matchContext)
            };

        return new[]
            {
                fighterMatchPoints,
                opponentMatchPoints
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
