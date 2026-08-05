namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;

/// <summary>
/// A Bounty tournament never stores "the current champion" - it's always derived by replaying the
/// pool's finished challenges in order, starting from <see cref="TournamentEntity.StartingChampionId"/>.
/// Used by <see cref="Services.TournamentService"/> for <see cref="Models.BountyState.DefenseCount"/> -
/// the champion/bank themselves are read directly off the tournament's <see cref="RatingCalculators.BountyRatingCalculator"/>
/// pool row, since there's no persisted counter for defenses.
/// </summary>
public static class BountyChampionship
{
    public readonly record struct State(Guid? ChampionHeroId, int DefenseCount);

    public static State Compute(Guid? startingChampionId, IReadOnlyList<MatchEntity> matches)
    {
        var championId = startingChampionId;
        var defenseCount = 0;

        foreach (var match in matches.Where(m => !m.IsPlanned).OrderBy(m => m.Date))
        {
            var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);
            if (winner is null)
            {
                continue;
            }

            if (winner.HeroId == championId)
            {
                defenseCount++;
            }
            else
            {
                championId = winner.HeroId;
                defenseCount = 0;
            }
        }

        return new State(championId, defenseCount);
    }
}
