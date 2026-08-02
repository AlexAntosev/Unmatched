namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Enums;

/// <summary>Generators are stateless pure logic (no catalog/player/EF dependencies), so - mirroring
/// GameModeValidatorFactory - they're constructed directly rather than resolved from the container.</summary>
public class TournamentFormatGeneratorFactory : ITournamentFormatGeneratorFactory
{
    public ITournamentFormatGenerator? TryCreate(TournamentFormat format) => format switch
    {
        TournamentFormat.SingleElimination => new SingleEliminationGenerator(),
        TournamentFormat.Swiss => new SwissGenerator(),
        TournamentFormat.GroupStage => new GroupStageGenerator(),
        _ => null
    };
}
