namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;

/// <summary>What completing a tournament produces: the self-funded award rows (see
/// <see cref="TournamentAwardScheduler"/>) and each participant's final standing, keyed by hero id.
/// <see cref="FinalPlacements"/> may map a hero to null - a GroupStage participant who never left the
/// groups has no meaningful final rank.</summary>
public record TournamentCompletionSchedule(
    IReadOnlyList<TournamentAwardEntity> Awards,
    IReadOnlyDictionary<Guid, int?> FinalPlacements);
