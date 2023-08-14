namespace Unmatched.Services;

using Unmatched.Entities;
using Unmatched.Repositories;

public class RatingCalculator : IRatingCalculator
{
    private readonly IHeroRepository _heroRepository;

    private readonly IRatingRepository _ratingRepository;

    public RatingCalculator(IRatingRepository ratingRepository, IHeroRepository heroRepository)
    {
        _ratingRepository = ratingRepository;
        _heroRepository = heroRepository;
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
            await _heroRepository.GetByIdAsync(winnerFighter.HeroId),
            await _heroRepository.GetByIdAsync(looserFighter.HeroId),
            winnerFighter,
            looserFighter,
            (await _ratingRepository.GetByHeroIdAsync(winnerFighter.HeroId))?.Points ?? 0,
            (await _ratingRepository.GetByHeroIdAsync(looserFighter.HeroId))?.Points ?? 0);

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
        return matchContext.LooserPointsBeforeMatch + result > 0
            ? result
            : -matchContext.LooserPointsBeforeMatch;
    }

    private int CalculateForWinner(MatchContext matchContext)
    {
        var maxLooserSidekicksHp = matchContext.LooserReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;
        var maxWinnerSidekicksHp = matchContext.WinnerReferenceHero.Sidekicks?.Sum(x => x.Hp * x.Count) ?? 0;

        var forLooserSidekick = maxLooserSidekicksHp == 0
            ? 40
            : (int)Math.Round(40.0 * (1.0 - Math.Round((double)matchContext.LooserFighter.SidekickHpLeft / maxLooserSidekicksHp, 1, MidpointRounding.ToPositiveInfinity)), 0);

        var forCardsLeft = Convert.ToInt32(
            80 * Math.Round(matchContext.WinnerFighter.CardsLeft / (0.8 * matchContext.WinnerReferenceHero.DeckSize), 1, MidpointRounding.ToPositiveInfinity));
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
