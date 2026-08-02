namespace Unmatched.MatchService.Domain.Enums;

/// <summary>
/// Which title kinds a tournament awards on completion (see Phase 5's title engine) - chosen with
/// checkboxes at creation and persisted per <see cref="Entities.TournamentTitleEntity"/>. Champion is
/// mandatory and always included regardless of what the create request asked for.
/// </summary>
public enum TournamentTitleKind
{
    Champion,
    RunnerUp,
    IronChin,
    CardShark,
    Executioner,
    Cinderella
}
