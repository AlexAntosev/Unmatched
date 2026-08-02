namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Beat an opponent whose rating was at least <see cref="TitleThresholds.GiantSlayerRatingGap"/>
/// points higher going into the match. Evaluated after the match has already been scored, so the
/// pre-match rating is reconstructed as current rating minus this match's own delta - MatchPoints is
/// exactly that delta, already stored on the fighter by the time titles are evaluated.</summary>
public class GiantSlayerTitleRule(IUnitOfWork unitOfWork) : ITitleRule
{
    public string RuleKey => Titles.GiantSlayer;

    public TitleExclusivity Exclusivity => TitleExclusivity.Shared;

    public async Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
    {
        var winners = match.Fighters.Where(f => f.IsWinner).ToList();
        var losers = match.Fighters.Where(f => !f.IsWinner).ToList();
        if (winners.Count == 0 || losers.Count == 0)
        {
            return new Dictionary<Guid, double?>();
        }

        var qualifiers = new Dictionary<Guid, double?>();
        foreach (var winner in winners)
        {
            var winnerPreMatchRating = await PreMatchRatingAsync(winner);

            // The largest gap against any qualifying loser is the metric - a winner facing several
            // opponents (team/FFA) gets credit for the toughest one, not just the first one checked.
            var largestGap = 0;
            foreach (var loser in losers)
            {
                var loserPreMatchRating = await PreMatchRatingAsync(loser);
                var gap = loserPreMatchRating - winnerPreMatchRating;
                if (gap >= TitleThresholds.GiantSlayerRatingGap && gap > largestGap)
                {
                    largestGap = gap;
                }
            }

            if (largestGap > 0)
            {
                qualifiers[winner.HeroId] = largestGap;
            }
        }

        return qualifiers;
    }

    private async Task<int> PreMatchRatingAsync(FighterEntity fighter)
        => await HeroRatingReader.GetRatingAsync(unitOfWork, fighter.HeroId) - (fighter.MatchPoints ?? 0);
}
