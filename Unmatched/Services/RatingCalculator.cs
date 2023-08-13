using Unmatched.Entities;
using Unmatched.Repositories;

namespace Unmatched.Services;

public class RatingCalculator : IRatingCalculator
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IHeroRepository _heroRepository;

    public RatingCalculator(IRatingRepository ratingRepository, IHeroRepository heroRepository)
    {
        _ratingRepository = ratingRepository;
        _heroRepository = heroRepository;
    }
    public async Task<IEnumerable<HeroMatchPoints>> CalculateAsync(Fighter fighter, Fighter opponent)
    {
        var winnerFighter = fighter.IsWinner ? fighter : opponent;
        var looserFighter = fighter.IsWinner ? opponent : fighter;
        
        var matchContext = new MatchContext(
            await _heroRepository.GetByIdAsync(winnerFighter.HeroId),
            await _heroRepository.GetByIdAsync(looserFighter.HeroId), 
            winnerFighter, 
            looserFighter,
            (await _ratingRepository.GetByHeroIdAsync(winnerFighter.HeroId))?.Points ?? 0,
            (await _ratingRepository.GetByHeroIdAsync(looserFighter.HeroId))?.Points ?? 0);
        
        var fighterMatchPoints = new HeroMatchPoints()
        {
            HeroId = winnerFighter.HeroId,
            Points = CalculateForWinner(matchContext)
        };
        
        var opponentMatchPoints = new HeroMatchPoints()
        {
            HeroId = looserFighter.HeroId,
            Points = CalculateForLooser(matchContext)
        };

        return new[] { fighterMatchPoints, opponentMatchPoints };
    }

    private int CalculateForLooser(MatchContext matchContext)
    {
        int forSidekickHp;
        int forWinnerHpLeft;
        var result = -(100 + forSidekickHp + forWinnerHpLeft);
        return matchContext.LooserPointsBeforeMatch + result > 0 ? result : 0;
    }

    private int CalculateForWinner(MatchContext matchContext)
    {
        int forLooserSidekick;
        int forCardsLeft;
        int forHandicap;
        int forHpLeft;
        var result = 200 + forLooserSidekick + forCardsLeft + forHandicap + forHpLeft;
        return result;
    }
}