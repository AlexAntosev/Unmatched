namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;

/// <summary>
/// The original bracket logic from TournamentService.CreateNextStagePlannedMatchesAsync, extracted
/// behind the generator contract. Round 1 draws from the tournament's participants (not the whole
/// catalog, unlike before); every later round advances the previous stage's winners, with two
/// exceptions the enum order encodes: SemiFinals' *losers* feed ThirdPlaceDecider, and GrandFinals is
/// seeded from SemiFinals' winners (not ThirdPlaceDecider's) since the third-place match doesn't gate
/// the final. GrandFinals is generated as three identical pairings - a temporary stand-in for a real
/// best-of-three.
/// </summary>
public class SingleEliminationGenerator : ITournamentFormatGenerator
{
    public bool CanGenerateNext(TournamentEntity tournament, IReadOnlyList<MatchEntity> existingMatches)
        // Grand finals is always the last stage generated - once it exists, there is nothing further.
        => existingMatches.Any(m => m.Stage == Stage.GrandFinals) == false;

    public (IReadOnlyList<TournamentPairing> Pairings, Stage? Stage, int? Round) GenerateNext(
        TournamentEntity tournament,
        IReadOnlyList<TournamentParticipantEntity> participants,
        IReadOnlyList<MatchEntity> existingMatches)
    {
        List<Guid> heroIds;
        Stage nextStage;

        if (existingMatches.Count == 0)
        {
            heroIds = participants.Select(p => p.HeroId).ToList();
            nextStage = tournament.CurrentStage;
        }
        else
        {
            heroIds = tournament.CurrentStage switch
            {
                Stage.SemiFinals => WinnersOrLosersOf(existingMatches, tournament.CurrentStage, wantWinners: false),
                Stage.ThirdPlaceDecider => WinnersOrLosersOf(existingMatches, tournament.CurrentStage - 1, wantWinners: true),
                _ => WinnersOrLosersOf(existingMatches, tournament.CurrentStage, wantWinners: true)
            };
            nextStage = tournament.CurrentStage + 1;
        }

        var pairings = Pair(heroIds.Shuffle());

        // temp solution for bo3 grand final
        if (nextStage == Stage.GrandFinals && pairings.Count == 1)
        {
            var grandFinal = pairings[0];
            pairings = [grandFinal, grandFinal, grandFinal];
        }

        return (pairings, nextStage, null);
    }

    private static List<Guid> WinnersOrLosersOf(IReadOnlyList<MatchEntity> matches, Stage stage, bool wantWinners)
        => matches
            .Where(m => m.Stage == stage)
            .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner == wantWinners))
            .Where(f => f is not null)
            .Select(f => f!.HeroId)
            .ToList();

    private static List<TournamentPairing> Pair(List<Guid> heroIds)
    {
        var pairings = new List<TournamentPairing>();
        for (var i = 0; i + 1 < heroIds.Count; i += 2)
        {
            pairings.Add(new TournamentPairing(heroIds[i], heroIds[i + 1]));
        }

        return pairings;
    }
}
