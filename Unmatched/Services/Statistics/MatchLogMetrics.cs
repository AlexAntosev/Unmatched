namespace Unmatched.Services.Statistics;

using System.Globalization;

using Unmatched.Dtos;

/// <summary>
/// The four figures above the match log. They are derived from the log the page already holds
/// rather than fetched separately, so they always agree with the rows underneath them.
/// </summary>
public record MatchLogMetrics(
    int TotalMatches,
    int AddedThisWeek,
    int MatchesThisMonth,
    int SessionsThisMonth,
    double AverageEpic,
    HeroStreak? LongestStreak)
{
    public static MatchLogMetrics Empty { get; } = new(0, 0, 0, 0, 0, null);

    public static MatchLogMetrics Calculate(IEnumerable<UiMatchLogDto> matches, DateTime today)
    {
        var all = matches.ToList();
        if (all.Count == 0)
        {
            return Empty;
        }

        var weekStart = today.Date.AddDays(-7);
        var monthMatches = all.Where(m => m.Date.Year == today.Year && m.Date.Month == today.Month).ToList();
        var rated = all.Where(m => m.Epic is > 0).Select(m => m.Epic!.Value).ToList();

        return new MatchLogMetrics(
            all.Count,
            all.Count(m => m.Date >= weekStart),
            monthMatches.Count,
            // A session is an evening of play, so same-day matches count once.
            monthMatches.Select(m => m.Date.Date).Distinct().Count(),
            rated.Count == 0 ? 0 : Math.Round(rated.Average(), 1),
            LongestHeroStreak(all));
    }

    /// <summary>
    /// The longest run of consecutive wins by one hero, using the same definition as the match
    /// service's Streak title: fighters ordered by date, reset on any loss.
    /// </summary>
    private static HeroStreak? LongestHeroStreak(IReadOnlyCollection<UiMatchLogDto> matches)
    {
        HeroStreak? best = null;

        var appearancesByHero = matches
            .SelectMany(match => match.Fighters.Select(fighter => (match.Date, Fighter: fighter)))
            .Where(x => x.Fighter.Hero is not null)
            .GroupBy(x => x.Fighter.Hero!.Name);

        foreach (var hero in appearancesByHero)
        {
            var run = 0;
            foreach (var appearance in hero.OrderBy(x => x.Date))
            {
                if (!appearance.Fighter.IsWinner)
                {
                    run = 0;
                    continue;
                }

                run++;
                if (best is null || run > best.Length)
                {
                    best = new HeroStreak(hero.Key, run, appearance.Date);
                }
            }
        }

        return best;
    }
}

/// <summary>A hero's best winning run and the date it ended, shown as "Medusa · 12 June 2026".</summary>
public record HeroStreak(string HeroName, int Length, DateTime EndedOn)
{
    public string Caption => $"{HeroName} · {EndedOn.ToString("d MMMM yyyy", CultureInfo.InvariantCulture)}";
}
