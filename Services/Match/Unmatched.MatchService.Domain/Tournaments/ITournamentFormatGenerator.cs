namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>
/// Decides who plays whom next for one tournament format. Deliberately pure/synchronous - no catalog,
/// player or map lookups here, so pairing logic (the part worth unit-testing in isolation) never
/// needs mocks. <see cref="Services.TournamentService"/> turns the returned pairings into actual
/// planned <see cref="Models.Match"/> objects (heroes, players, maps).
/// </summary>
public interface ITournamentFormatGenerator
{
    /// <summary>Whether there is a next round/stage to generate at all, independent of whether the
    /// current one has finished (that check lives in TournamentService, uniformly for every format).</summary>
    bool CanGenerateNext(TournamentEntity tournament, IReadOnlyList<MatchEntity> existingMatches);

    /// <returns>The pairings for the next round, plus the bracket <see cref="Stage"/> they belong to
    /// (SingleElimination and the playoff phase of GroupStage) or the <see cref="MatchEntity.Round"/>
    /// number (Swiss and the group phase of GroupStage) - exactly one of the two is set.</returns>
    (IReadOnlyList<TournamentPairing> Pairings, Stage? Stage, int? Round) GenerateNext(
        TournamentEntity tournament,
        IReadOnlyList<TournamentParticipantEntity> participants,
        IReadOnlyList<MatchEntity> existingMatches);
}
