namespace Unmatched.MatchService.Domain.Enums;

/// <summary>How many games decide a SingleElimination tournament's grand final. Bo3 is scheduled
/// two games at a time - see <see cref="Tournaments.SingleEliminationGenerator"/> - so a 2-0 sweep
/// never generates a pointless third, already-decided game.</summary>
public enum TournamentFinalFormat
{
    Bo1 = 0,
    Bo3 = 1
}
