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

    public async Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        var winners = match.Fighters.Where(f => f.IsWinner).ToList();
        var losers = match.Fighters.Where(f => !f.IsWinner).ToList();
        if (winners.Count == 0 || losers.Count == 0)
        {
            return new HashSet<Guid>();
        }

        var qualifiers = new HashSet<Guid>();
        foreach (var winner in winners)
        {
            var winnerPreMatchRating = await PreMatchRatingAsync(winner);
            foreach (var loser in losers)
            {
                var loserPreMatchRating = await PreMatchRatingAsync(loser);
                if (loserPreMatchRating - winnerPreMatchRating >= TitleThresholds.GiantSlayerRatingGap)
                {
                    qualifiers.Add(winner.HeroId);
                    break;
                }
            }
        }

        return qualifiers;
    }

    private async Task<int> PreMatchRatingAsync(FighterEntity fighter)
        => await HeroRatingReader.GetRatingAsync(unitOfWork, fighter.HeroId) - (fighter.MatchPoints ?? 0);
}
