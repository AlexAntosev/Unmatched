namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;

/// <summary>One opponent a hero has faced, with how that pairing has gone.</summary>
public record HeroMatchup(Guid OpponentHeroId, string OpponentName, string OpponentImageUrl, int Wins, int Losses)
{
    public int Total => Wins + Losses;

    public int WinPercent => Total == 0 ? 0 : (int)Math.Round((double)Wins / Total * 100);

    public double Kd => Losses == 0 ? Wins : Math.Round((double)Wins / Losses, 2);
}

/// <summary>
/// Turns a hero's match log into the per-opponent records the hero page and the ladder's spotlight
/// column show. Lives here rather than in a page so both screens compute them the same way.
/// </summary>
public static class HeroMatchupCalculator
{
    private record Record(string Name, string ImageUrl, int Wins, int Losses);

    public static IReadOnlyList<HeroMatchup> Calculate(Guid heroId, IEnumerable<UiMatchLogDto> matches)
    {
        var opponents = new Dictionary<Guid, Record>();

        foreach (var match in matches)
        {
            var fighters = match.Fighters.ToList();
            var self = fighters.FirstOrDefault(f => f.HeroId == heroId);
            if (self is null)
            {
                continue;
            }

            foreach (var opponent in fighters.Where(f => !IsSameSide(self, f)))
            {
                if (opponent.Hero is null)
                {
                    continue;
                }

                var current = opponents.GetValueOrDefault(
                    opponent.HeroId,
                    new Record(opponent.Hero.Name, opponent.HeroImageUrl, 0, 0));

                opponents[opponent.HeroId] = self.IsWinner
                    ? current with { Wins = current.Wins + 1 }
                    : current with { Losses = current.Losses + 1 };
            }
        }

        return opponents
            .Select(pair => new HeroMatchup(pair.Key, pair.Value.Name, pair.Value.ImageUrl, pair.Value.Wins, pair.Value.Losses))
            .OrderByDescending(m => m.WinPercent)
            .ThenByDescending(m => m.Total)
            .ToList();
    }

    /// <summary>The hero this page is about, and in team games their team mates, are not opponents.</summary>
    private static bool IsSameSide(UiFighterDto self, UiFighterDto other)
        => other.HeroId == self.HeroId
        || (self.Team is not null && other.Team == self.Team);

    /// <summary>The opponent met most often, which the hero page calls the antagonist.</summary>
    public static HeroMatchup? Antagonist(IReadOnlyList<HeroMatchup> matchups)
        => matchups
            .OrderByDescending(m => m.Total)
            .ThenByDescending(m => m.Losses)
            .FirstOrDefault();

    /// <summary>
    /// The strongest and weakest pairings rather than the top of the list, so the panel shows a
    /// spread instead of a run of identical percentages. Input is expected sorted by win rate.
    /// </summary>
    public static IReadOnlyList<HeroMatchup> BestAndWorst(IReadOnlyList<HeroMatchup> matchups, int best = 3, int worst = 2)
    {
        if (matchups.Count <= best + worst)
        {
            return matchups;
        }

        return matchups.Take(best).Concat(matchups.TakeLast(worst)).ToList();
    }
}
