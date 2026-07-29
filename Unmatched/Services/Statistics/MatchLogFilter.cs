namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;
using Unmatched.Enums;

/// <summary>
/// The filter strip on the match log. Applied in memory over the log the page already loaded -
/// at the volumes this app sees that is cheaper than a round trip per keystroke. Every dropdown is
/// multi-select (design/IMPLEMENTATION-PROMPT.md §2): an empty set means "don't filter on this",
/// a non-empty one means "match any of these".
/// </summary>
public class MatchLogFilter
{
    public GameMode? Mode { get; set; }

    public HashSet<Guid> PlayerIds { get; set; } = [];

    public HashSet<Guid> HeroIds { get; set; } = [];

    public HashSet<string> MapNames { get; set; } = [];

    public HashSet<string> TournamentNames { get; set; } = [];

    /// <summary>Selected range presets in days (e.g. 30/90/365) - OR'd together, so only the
    /// loosest (largest) one actually constrains anything.</summary>
    public HashSet<int> RangeDays { get; set; } = [];

    /// <summary>Free text matched against hero, player and map names.</summary>
    public string? Search { get; set; }

    public bool IsEmpty
        => Mode is null && PlayerIds.Count == 0 && HeroIds.Count == 0
        && MapNames.Count == 0 && TournamentNames.Count == 0
        && RangeDays.Count == 0 && string.IsNullOrWhiteSpace(Search);

    public void Clear()
    {
        Mode = null;
        PlayerIds.Clear();
        HeroIds.Clear();
        MapNames.Clear();
        TournamentNames.Clear();
        RangeDays.Clear();
    }

    public IEnumerable<UiMatchLogDto> Apply(IEnumerable<UiMatchLogDto> matches)
    {
        var filtered = matches;

        if (Mode is not null)
        {
            filtered = filtered.Where(m => m.GameMode == Mode);
        }

        if (PlayerIds.Count > 0)
        {
            filtered = filtered.Where(m => m.Fighters.Any(f => PlayerIds.Contains(f.PlayerId)));
        }

        if (HeroIds.Count > 0)
        {
            filtered = filtered.Where(m => m.Fighters.Any(f => HeroIds.Contains(f.HeroId)));
        }

        if (MapNames.Count > 0)
        {
            filtered = filtered.Where(m => MapNames.Contains(m.MapName));
        }

        if (TournamentNames.Count > 0)
        {
            filtered = filtered.Where(m => TournamentNames.Contains(m.TournamentName));
        }

        if (RangeDays.Count > 0)
        {
            var from = DateTime.Today.AddDays(-RangeDays.Max());
            filtered = filtered.Where(m => m.Date.Date >= from);
        }

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = Search.Trim();
            filtered = filtered.Where(m => Matches(m, term));
        }

        return filtered;
    }

    private static bool Matches(UiMatchLogDto match, string term)
    {
        if (Contains(match.MapName, term) || Contains(match.TournamentName, term) || Contains(match.Villain?.Name, term))
        {
            return true;
        }

        return match.Fighters.Any(f => Contains(f.Hero?.Name, term) || Contains(f.Player?.Name, term));
    }

    private static bool Contains(string? value, string term)
        => value is not null && value.Contains(term, StringComparison.OrdinalIgnoreCase);
}
