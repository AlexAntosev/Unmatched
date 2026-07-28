namespace Unmatched.UI.BlazorServer.Shared.DesignSystem;

using Unmatched.Enums;

/// <summary>
/// The label, icon and hue each game mode carries in the design. Kept in one place because the
/// same values drive the mode badge, the match row's left edge and the mode filter segments.
/// </summary>
public record GameModeStyle(string Label, string Icon, string Color, string Background)
{
    private static readonly GameModeStyle OneVsOne = new("1V1", "bi-person-fill", "var(--um-mode-1v1)", "var(--um-mode-1v1-bg)");
    private static readonly GameModeStyle TeamVsTeam = new("2V2", "bi-people-fill", "var(--um-mode-2v2)", "var(--um-mode-2v2-bg)");
    private static readonly GameModeStyle FreeForAll = new("FFA", "bi-diagram-3-fill", "var(--um-mode-ffa)", "var(--um-mode-ffa-bg)");
    private static readonly GameModeStyle Cooperative = new("CO-OP", "bi-shield-fill", "var(--um-mode-coop)", "var(--um-mode-coop-bg)");

    public static GameModeStyle For(GameMode mode)
        => mode switch
        {
            GameMode.TeamVsTeam => TeamVsTeam,
            GameMode.FreeForAll => FreeForAll,
            GameMode.Cooperative => Cooperative,
            _ => OneVsOne
        };

    /// <summary>Short label for the compact mode column on the hero page's match log.</summary>
    public static string ShortLabel(GameMode mode)
        => mode switch
        {
            GameMode.TeamVsTeam => "2v2",
            GameMode.FreeForAll => "ffa",
            GameMode.Cooperative => "co-op",
            _ => "1v1"
        };
}
