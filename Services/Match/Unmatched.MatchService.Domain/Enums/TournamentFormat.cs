namespace Unmatched.MatchService.Domain.Enums;

/// <summary>
/// Int values are chosen to line up with the old TournamentType (League = 0, Championship = 1) so the
/// migration renaming that column is a pure rename with no data update.
/// </summary>
public enum TournamentFormat
{
    League = 0,
    SingleElimination = 1,
    GroupStage = 2,
    Swiss = 3,
    Bounty = 4
}
