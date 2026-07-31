namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Unmatched.MatchService.Domain.Enums;

/// <summary>One row per title kind a tournament will award on completion - which kinds are chosen at
/// creation time (see <see cref="TournamentEntity.TournamentTitles"/>).</summary>
[Table("TournamentTitles")]
public class TournamentTitleEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public TournamentTitleKind Kind { get; set; }
}
