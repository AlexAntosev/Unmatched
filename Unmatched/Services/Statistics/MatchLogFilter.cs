namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;
using Unmatched.Enums;

/// <summary>
/// The filter strip on the match log. Applied in memory over the log the page already loaded -
/// at the volumes this app sees that is cheaper than a round trip per keystroke.
/// </summary>
public class MatchLogFilter
{
    public GameMode? Mode { get; set; }

    public Guid? PlayerId { get; set; }

    public Guid? HeroId { get; set; }

    public string? MapName { get; set; }

    public string? TournamentName { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    /// <summary>Free text matched against hero, player and map names.</summary>
    public string? Search { get; set; }

    public bool IsEmpty
        => Mode is null && PlayerId is null && HeroId is null
        && string.IsNullOrWhiteSpace(MapName) && string.IsNullOrWhiteSpace(TournamentName)
        && From is null && To is null && string.IsNullOrWhiteSpace(Search);

    public IEnumerable<UiMatchLogDto> Apply(IEnumerable<UiMatchLogDto> matches)
    {
        var filtered = matches;

        if (Mode is not null)
        {
            filtered = filtered.Where(m => m.GameMode == Mode);
        }

        if (PlayerId is not null)
        {
            filtered = filtered.Where(m => m.Fighters.Any(f => f.PlayerId == PlayerId));
        }

        if (HeroId is not null)
        {
            filtered = filtered.Where(m => m.Fighters.Any(f => f.HeroId == HeroId));
        }

        if (!string.IsNullOrWhiteSpace(MapName))
        {
            filtered = filtered.Where(m => m.MapName == MapName);
        }

        if (!string.IsNullOrWhiteSpace(TournamentName))
        {
            filtered = filtered.Where(m => m.TournamentName == TournamentName);
        }

        if (From is not null)
        {
            filtered = filtered.Where(m => m.Date.Date >= From.Value.Date);
        }

        if (To is not null)
        {
            filtered = filtered.Where(m => m.Date.Date <= To.Value.Date);
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
