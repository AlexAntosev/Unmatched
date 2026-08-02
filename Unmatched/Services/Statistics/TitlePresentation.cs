namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;
using Unmatched.Enums;

/// <summary>The color category a title's icon/border renders in - Achievement for every Shared title,
/// Prestige/Neutral/Negative splitting the Unique titles by how good a thing it is to hold one.</summary>
public enum TitleAccent
{
    Achievement,
    Prestige,
    Neutral,
    Negative
}

/// <summary>A title's icon and accent, looked up by RuleKey (automatic titles) or Kind (tournament
/// titles) rather than by Name - manual, non-rule-backed titles fall back to a generic look.</summary>
public record TitleStyle(string Icon, TitleAccent Accent)
{
    private static readonly TitleStyle SharedFallback = new("bi-patch-check-fill", TitleAccent.Achievement);
    private static readonly TitleStyle UniqueFallback = new("bi-award-fill", TitleAccent.Neutral);

    private static readonly IReadOnlyDictionary<string, TitleStyle> ByRuleKey = new Dictionary<string, TitleStyle>
    {
        ["flawless"] = new("bi-patch-check-fill", TitleAccent.Achievement),
        ["last-breath"] = new("bi-heartbreak-fill", TitleAccent.Achievement),
        ["giant-slayer"] = new("bi-graph-up-arrow", TitleAccent.Achievement),
        ["rusher"] = new("bi-stopwatch-fill", TitleAccent.Achievement),
        ["punisher"] = new("bi-fire", TitleAccent.Achievement),
        ["deck-miller"] = new("bi-layers-fill", TitleAccent.Achievement),
        ["grand-champion"] = new("bi-trophy-fill", TitleAccent.Prestige),
        ["streak"] = new("bi-lightning-charge-fill", TitleAccent.Prestige),
        ["kingslayer"] = new("bi-crosshair", TitleAccent.Prestige),
        ["sufferer"] = new("bi-emoji-frown-fill", TitleAccent.Negative),
        ["wall"] = new("bi-shield-shaded", TitleAccent.Neutral),
        ["workhorse"] = new("bi-hourglass-split", TitleAccent.Neutral),
        ["executioner"] = new("bi-scissors", TitleAccent.Neutral),
        ["bounty-holder"] = new("bi-coin", TitleAccent.Neutral)
    };

    private static readonly IReadOnlyDictionary<TournamentTitleKind, TitleStyle> ByTournamentKind = new Dictionary<TournamentTitleKind, TitleStyle>
    {
        [TournamentTitleKind.Champion] = new("bi-trophy-fill", TitleAccent.Prestige),
        [TournamentTitleKind.RunnerUp] = new("bi-award", TitleAccent.Neutral),
        [TournamentTitleKind.IronChin] = new("bi-shield-fill", TitleAccent.Neutral),
        [TournamentTitleKind.CardShark] = new("bi-suit-spade-fill", TitleAccent.Neutral),
        [TournamentTitleKind.Executioner] = new("bi-scissors", TitleAccent.Neutral),
        [TournamentTitleKind.Cinderella] = new("bi-stars", TitleAccent.Neutral)
    };

    public static TitleStyle For(TitleDto title)
    {
        if (title.Kind is { } kind && ByTournamentKind.TryGetValue(kind, out var tournamentStyle))
        {
            return tournamentStyle;
        }

        if (title.RuleKey is not null && ByRuleKey.TryGetValue(title.RuleKey, out var ruleStyle))
        {
            return ruleStyle;
        }

        return title.Exclusivity == TitleExclusivity.Shared ? SharedFallback : UniqueFallback;
    }
}

/// <summary>A title bundled with the icon/accent it renders with - the shared building block every
/// view-model below is built from.</summary>
public record TitleEntry(TitleDto Title, TitleStyle Style)
{
    /// <summary>The bare kind label ("Champion") for a tournament title rather than the full string
    /// TournamentTitleAwarder bakes into Name ("Champion of Winter Cup 2026") - the tournament name
    /// already renders alongside it wherever a tournament title is shown, so repeating it here would
    /// just be noise. Falls back to Name for every other title.</summary>
    public string DisplayName => Title.Kind is { } kind ? KindLabel(kind) : Title.Name ?? "Untitled";

    /// <summary>Formats a holder's stored <see cref="TitleHolderDto.Metric"/> in the unit this title's
    /// rule uses (e.g. "214 sidekick HP") - null when this rule has no such number or none was stored
    /// for this holder (see <see cref="TitleRuleMetricFormat"/> for which rules do).</summary>
    public string? MetricLabel(double? metric)
        => metric is null || Title.RuleKey is null
            ? null
            : TitleRuleMetricFormat.ByRuleKey.GetValueOrDefault(Title.RuleKey)?.Invoke(metric.Value);

    private static string KindLabel(TournamentTitleKind kind) => kind switch
    {
        TournamentTitleKind.Champion => "Champion",
        TournamentTitleKind.RunnerUp => "Runner-Up",
        TournamentTitleKind.IronChin => "Iron Chin",
        TournamentTitleKind.CardShark => "Card Shark",
        TournamentTitleKind.Executioner => "Executioner",
        TournamentTitleKind.Cinderella => "Cinderella",
        _ => kind.ToString()
    };
}

/// <summary>How to print the "how much/how many" value each metric-carrying rule stores (see
/// Domain/Titles/Rules) - only the rules with a natural single number are listed; every other RuleKey
/// falls back to no label even if a value somehow got stored.</summary>
internal static class TitleRuleMetricFormat
{
    public static readonly IReadOnlyDictionary<string, Func<double, string>> ByRuleKey = new Dictionary<string, Func<double, string>>
    {
        ["rusher"] = v => $"{(int)v} cards left",
        ["punisher"] = v => $"+{(int)v} rating",
        ["last-breath"] = v => $"{(int)v} HP left",
        ["giant-slayer"] = v => $"{(int)v} rating gap",
        ["grand-champion"] = v => $"{(int)v} rating",
        ["streak"] = v => $"{(int)v} wins in a row",
        ["sufferer"] = v => $"{(int)v} losses in a row",
        ["executioner"] = v => $"{(int)v} sidekick HP",
        ["wall"] = v => $"{v:0.0} HP per win",
        ["workhorse"] = v => $"{(int)v} matches"
    };
}

public record HeldAchievement(TitleEntry Entry, int TimesEarned, DateTime? LastEarnedAt, double? Metric)
{
    public string? MetricLabel => Entry.MetricLabel(Metric);
}

public record HeldUniqueTitle(TitleEntry Entry, DateTime? HeldSince, double? Metric)
{
    public string? MetricLabel => Entry.MetricLabel(Metric);
}

public record HeldTournamentTitle(TitleEntry Entry, string TournamentName, DateTime? EarnedAt);

/// <summary>Everything the hero-page "Titles &amp; achievements" section needs for one hero - only what
/// the hero actually holds; titles/achievements it hasn't earned aren't listed here at all (the /titles
/// catalogue page is where the full picture, held or not, lives).</summary>
public record HeroTitlesSummary(
    IReadOnlyList<HeldUniqueTitle> HeldTitles,
    IReadOnlyList<HeldAchievement> HeldAchievements,
    IReadOnlyList<HeldTournamentTitle> TournamentTitles,
    int TitlesHeldCount,
    int AchievementsHeldCount,
    int AchievementsTotalCount);

public record AchievementCard(TitleEntry Entry, IReadOnlyList<TitleHolderDto> TopHolders, int TotalHolderCount, int TotalAwards);

/// <summary>One row of the Titles table - a null <see cref="Holder"/> means the title is vacant.</summary>
public record TitleTableRow(TitleEntry Entry, TitleHolderDto? Holder);

public record TournamentTitleGroup(TournamentDto Tournament, IReadOnlyList<TitleTableRow> Rows);

/// <summary>Everything the /titles catalogue page needs, already split into its three tabs.</summary>
public record TitlesCatalog(
    IReadOnlyList<AchievementCard> Achievements,
    IReadOnlyList<TitleTableRow> Titles,
    IReadOnlyList<TournamentTitleGroup> TournamentGroups,
    int AchievementsCount,
    int AchievementAwardsTotal,
    int TitlesHeldCount,
    int TitlesVacantCount,
    int TournamentTitlesCount,
    int TournamentsCount);

/// <summary>
/// Reshapes the flat title catalog (Shared = achievement, Unique = title, TournamentId != null =
/// tournament title - see TitleExclusivity) into the view-models the hero-page section and the /titles
/// catalogue page render, mirroring MatchPresentation's markup-free static-class pattern.
/// </summary>
public static class TitlePresentation
{
    private const int TopHoldersShown = 3;

    public static HeroTitlesSummary ForHero(Guid heroId, IEnumerable<TitleDto> catalog, IEnumerable<TournamentDto> tournaments)
    {
        var titles = catalog.ToList();
        var tournamentsById = tournaments.ToDictionary(t => t.Id);

        var uniqueTitles = titles.Where(t => t.Exclusivity == TitleExclusivity.Unique && t.TournamentId is null).ToList();
        var achievements = titles.Where(t => t.Exclusivity == TitleExclusivity.Shared).ToList();
        var tournamentTitles = titles.Where(t => t.TournamentId is not null).ToList();

        var heldTitles = uniqueTitles
            .Where(t => t.Holders.Any(h => h.HeroId == heroId))
            .Select(t =>
            {
                var holder = t.Holders.First(h => h.HeroId == heroId);
                return new HeldUniqueTitle(Entry(t), holder.EarnedAt, holder.Metric);
            })
            .ToList();

        var heldAchievements = achievements
            .Where(t => t.Holders.Any(h => h.HeroId == heroId))
            .Select(t =>
            {
                var holder = t.Holders.First(h => h.HeroId == heroId);
                return new HeldAchievement(Entry(t), holder.TimesEarned, holder.EarnedAt, holder.Metric);
            })
            .ToList();

        var heldTournamentTitles = tournamentTitles
            .Where(t => t.Holders.Any(h => h.HeroId == heroId))
            .Select(t =>
            {
                var holder = t.Holders.First(h => h.HeroId == heroId);
                return new HeldTournamentTitle(Entry(t), TournamentName(t, tournamentsById), holder.EarnedAt);
            })
            .OrderByDescending(t => t.EarnedAt)
            .ToList();

        return new HeroTitlesSummary(
            heldTitles,
            heldAchievements,
            heldTournamentTitles,
            TitlesHeldCount: heldTitles.Count,
            AchievementsHeldCount: heldAchievements.Count,
            AchievementsTotalCount: achievements.Count);
    }

    public static TitlesCatalog Catalog(IEnumerable<TitleDto> catalog, IEnumerable<TournamentDto> tournaments)
    {
        var titles = catalog.ToList();
        var tournamentsById = tournaments.ToDictionary(t => t.Id);

        var achievementCards = titles
            .Where(t => t.Exclusivity == TitleExclusivity.Shared)
            .Select(t => new AchievementCard(
                Entry(t),
                t.Holders.OrderByDescending(h => h.TimesEarned).ThenByDescending(h => h.EarnedAt).Take(TopHoldersShown).ToList(),
                t.Holders.Count(),
                t.Holders.Sum(h => h.TimesEarned)))
            .ToList();

        var titleRows = titles
            .Where(t => t.Exclusivity == TitleExclusivity.Unique && t.TournamentId is null)
            .Select(t => new TitleTableRow(Entry(t), t.Holders.FirstOrDefault()))
            .ToList();

        var tournamentGroups = titles
            .Where(t => t.TournamentId is not null && tournamentsById.ContainsKey(t.TournamentId!.Value))
            .GroupBy(t => t.TournamentId!.Value)
            .Select(g => new TournamentTitleGroup(
                tournamentsById[g.Key],
                g.OrderBy(t => KindOrder(t.Kind)).Select(t => new TitleTableRow(Entry(t), t.Holders.FirstOrDefault())).ToList()))
            .OrderByDescending(g => g.Tournament.CompletedAt)
            .ToList();

        return new TitlesCatalog(
            achievementCards,
            titleRows,
            tournamentGroups,
            AchievementsCount: achievementCards.Count,
            AchievementAwardsTotal: achievementCards.Sum(c => c.TotalAwards),
            TitlesHeldCount: titleRows.Count(r => r.Holder is not null),
            TitlesVacantCount: titleRows.Count(r => r.Holder is null),
            TournamentTitlesCount: tournamentGroups.Sum(g => g.Rows.Count),
            TournamentsCount: tournamentGroups.Count);
    }

    private static TitleEntry Entry(TitleDto title) => new(title, TitleStyle.For(title));

    private static string TournamentName(TitleDto title, IReadOnlyDictionary<Guid, TournamentDto> tournamentsById)
        => title.TournamentId is { } id && tournamentsById.TryGetValue(id, out var tournament) ? tournament.Name : "Unknown tournament";

    private static int KindOrder(TournamentTitleKind? kind) => kind switch
    {
        TournamentTitleKind.Champion => 0,
        TournamentTitleKind.RunnerUp => 1,
        TournamentTitleKind.IronChin => 2,
        TournamentTitleKind.CardShark => 3,
        TournamentTitleKind.Executioner => 4,
        TournamentTitleKind.Cinderella => 5,
        _ => 6
    };
}
