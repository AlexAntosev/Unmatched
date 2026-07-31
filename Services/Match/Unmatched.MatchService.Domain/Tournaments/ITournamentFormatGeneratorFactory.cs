namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Enums;

public interface ITournamentFormatGeneratorFactory
{
    /// <returns>Null for formats with no automatic match generation - League (matches are entered by
    /// hand) and Bounty (its challenge-generation flow is deferred).</returns>
    ITournamentFormatGenerator? TryCreate(TournamentFormat format);
}
