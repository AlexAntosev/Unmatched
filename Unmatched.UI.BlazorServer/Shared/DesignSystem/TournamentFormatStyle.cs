namespace Unmatched.UI.BlazorServer.Shared.DesignSystem;

using Unmatched.Enums;

/// <summary>The label and icon each tournament format carries, kept in one place because the same
/// values drive the format filter, the tournament cards and the create form.</summary>
public record TournamentFormatStyle(string Label, string Icon)
{
    private static readonly TournamentFormatStyle League = new("League", "bi-list-ol");
    private static readonly TournamentFormatStyle SingleElimination = new("Playoff", "bi-diagram-3-fill");
    private static readonly TournamentFormatStyle GroupStage = new("Group stage", "bi-grid-3x3-gap-fill");
    private static readonly TournamentFormatStyle Swiss = new("Swiss", "bi-shuffle");
    private static readonly TournamentFormatStyle Bounty = new("Bounty", "bi-bullseye");

    public static TournamentFormatStyle For(TournamentFormat format) => format switch
    {
        TournamentFormat.SingleElimination => SingleElimination,
        TournamentFormat.GroupStage => GroupStage,
        TournamentFormat.Swiss => Swiss,
        TournamentFormat.Bounty => Bounty,
        _ => League
    };

    /// <summary>Only these formats have a working match generator today - Bounty's is deferred and
    /// League is entered by hand, so "Generate matches" has nothing to do for either.</summary>
    public static bool SupportsGeneration(TournamentFormat format)
        => format is TournamentFormat.SingleElimination or TournamentFormat.GroupStage or TournamentFormat.Swiss;

    /// <summary>Which formats show the "Match saved" popup with a way back to the tournament, instead
    /// of navigating straight there - only the duel-style formats where one result is worth a beat
    /// before returning; League/GroupStage/Swiss go straight back to the bracket.</summary>
    public static bool ShowsMatchResultPopup(TournamentFormat format)
        => format is TournamentFormat.SingleElimination or TournamentFormat.Bounty;
}
