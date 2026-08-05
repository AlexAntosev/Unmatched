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
/// the final. GrandFinals is scheduled one or two games at a time depending on
/// <see cref="TournamentEntity.FinalFormat"/>: Bo1 is a single game; Bo3 generates two games up front
/// and, only if they split 1-1, a decider third once both are finished - a 2-0 sweep never generates a
/// pointless already-decided third game.
/// </summary>
public class SingleEliminationGenerator : ITournamentFormatGenerator
{
    public bool CanGenerateNext(TournamentEntity tournament, IReadOnlyList<MatchEntity> existingMatches)
    {
        var grandFinals = existingMatches.Where(m => m.Stage == Stage.GrandFinals).ToList();
        if (grandFinals.Count == 0)
        {
            return true;
        }

        return tournament.FinalFormat == TournamentFinalFormat.Bo3
            && grandFinals.Count == 2
            && IsTiedOneGameEach(grandFinals);
    }

    public (IReadOnlyList<TournamentPairing> Pairings, Stage? Stage, int? Round) GenerateNext(
        TournamentEntity tournament,
        IReadOnlyList<TournamentParticipantEntity> participants,
        IReadOnlyList<MatchEntity> existingMatches)
    {
        var existingGrandFinals = existingMatches.Where(m => m.Stage == Stage.GrandFinals).ToList();
        if (existingGrandFinals.Count > 0)
        {
            // The only reason GenerateNext is ever called again once GrandFinals exists is to schedule
            // the Bo3 decider game - CanGenerateNext gates everything else.
            var finalists = existingGrandFinals[0].Fighters.ToList();
            var decider = new TournamentPairing(finalists[0].HeroId, finalists[1].HeroId);
            return ([decider], Stage.GrandFinals, null);
        }

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

        if (nextStage == Stage.GrandFinals && pairings.Count == 1)
        {
            var grandFinal = pairings[0];
            var gameCount = tournament.FinalFormat == TournamentFinalFormat.Bo1 ? 1 : 2;
            pairings = Enumerable.Repeat(grandFinal, gameCount).ToList();
        }

        return (pairings, nextStage, null);
    }

    /// <summary>Both Bo3 games must be finished, one win each, before a decider is scheduled - a 2-0
    /// sweep already has a champion and needs no third game.</summary>
    private static bool IsTiedOneGameEach(List<MatchEntity> grandFinals)
    {
        if (grandFinals.Any(m => m.IsPlanned))
        {
            return false;
        }

        var winCounts = grandFinals
            .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner)?.HeroId)
            .Where(id => id.HasValue)
            .GroupBy(id => id!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        return winCounts.Count == 2 && winCounts.Values.All(wins => wins == 1);
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
