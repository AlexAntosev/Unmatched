namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;

/// <summary>
/// A Bounty tournament never stores "the current champion" - it's always derived by replaying the
/// pool's finished challenges in order, starting from <see cref="TournamentEntity.StartingChampionId"/>.
/// Shared by <see cref="Services.TournamentService"/> (who a new challenge is against) and
/// <see cref="BountyChallengeResolver"/> (who the dynamic holder title belongs to).
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
