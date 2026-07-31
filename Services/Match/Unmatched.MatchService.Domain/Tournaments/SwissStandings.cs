namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;

/// <summary>Score (wins) and Buchholz (sum of opponents' wins) per hero, from finished 1v1 matches -
/// the standard Swiss ranking, shared by <see cref="SwissGenerator"/> (pairing the next round) and
/// <see cref="TournamentAwardScheduler"/> (ranking the final standings for completion bonuses).</summary>
public static class SwissStandings
{
    public static List<(Guid HeroId, int Score, int Buchholz)> Compute(
        IEnumerable<Guid> heroIds, IReadOnlyList<MatchEntity> matches)
    {
        var heroIdList = heroIds.ToList();
        var wins = heroIdList.ToDictionary(id => id, _ => 0);
        var opponents = heroIdList.ToDictionary(id => id, _ => new List<Guid>());

        foreach (var match in matches.Where(m => !m.IsPlanned))
        {
            var fighters = match.Fighters.ToList();
            if (fighters.Count != 2)
            {
                continue;
            }

            var (a, b) = (fighters[0], fighters[1]);
            RecordResult(wins, opponents, a.HeroId, a.IsWinner, b.HeroId);
            RecordResult(wins, opponents, b.HeroId, b.IsWinner, a.HeroId);
        }

        return heroIdList
            .Select(id => (id, wins[id], opponents[id].Sum(opponent => wins.GetValueOrDefault(opponent))))
            .ToList();
    }

    private static void RecordResult(Dictionary<Guid, int> wins, Dictionary<Guid, List<Guid>> opponents, Guid heroId, bool isWinner, Guid opponentId)
    {
        if (!wins.ContainsKey(heroId))
        {
            return;
        }

        if (isWinner)
        {
            wins[heroId]++;
        }

        opponents[heroId].Add(opponentId);
    }
}
