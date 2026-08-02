namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>
/// Round-based pairing by standing: each round pairs the highest-scoring hero still unpaired against
/// the next-highest one it hasn't already played, working down the field. Score is wins so far;
/// Buchholz (the sum of a hero's opponents' scores) breaks ties, the standard Swiss tiebreak. There is
/// no natural "final round" a Swiss tournament can detect on its own - the organiser decides when to
/// stop and completes the tournament (Phase 4) whenever they're satisfied.
/// </summary>
public class SwissGenerator : ITournamentFormatGenerator
{
    public bool CanGenerateNext(TournamentEntity tournament, IReadOnlyList<MatchEntity> existingMatches) => true;

    public (IReadOnlyList<TournamentPairing> Pairings, Stage? Stage, int? Round) GenerateNext(
        TournamentEntity tournament,
        IReadOnlyList<TournamentParticipantEntity> participants,
        IReadOnlyList<MatchEntity> existingMatches)
    {
        var heroIds = participants.Select(p => p.HeroId).ToList();
        var nextRound = existingMatches.Select(m => m.Round ?? 0).DefaultIfEmpty(0).Max() + 1;

        var standings = SwissStandings.Compute(heroIds, existingMatches);
        var alreadyPlayed = PlayedPairs(existingMatches);

        var remaining = standings
            .OrderByDescending(s => s.Score)
            .ThenByDescending(s => s.Buchholz)
            .Select(s => s.HeroId)
            .ToList();

        var pairings = new List<TournamentPairing>();
        while (remaining.Count >= 2)
        {
            var heroId = remaining[0];
            remaining.RemoveAt(0);

            var opponentIndex = remaining.FindIndex(candidate => !HasPlayed(alreadyPlayed, heroId, candidate));
            if (opponentIndex < 0)
            {
                // every remaining hero has already faced this one at least once - a rematch is
                // unavoidable in a small field, so just take the next-best standing.
                opponentIndex = 0;
            }

            var opponentId = remaining[opponentIndex];
            remaining.RemoveAt(opponentIndex);
            pairings.Add(new TournamentPairing(heroId, opponentId));
        }

        // remaining.Count == 1 here means an odd field - that hero sits out this round (a bye).
        return (pairings, null, nextRound);
    }

    private static HashSet<(Guid, Guid)> PlayedPairs(IReadOnlyList<MatchEntity> matches)
        => matches
            .Select(m => m.Fighters.Select(f => f.HeroId).ToList())
            .Where(pair => pair.Count == 2)
            .Select(pair => (pair[0], pair[1]))
            .ToHashSet();

    private static bool HasPlayed(HashSet<(Guid, Guid)> playedPairs, Guid a, Guid b)
        => playedPairs.Contains((a, b)) || playedPairs.Contains((b, a));
}
