namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("TournamentParticipants")]
public class TournamentParticipantEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Guid HeroId { get; set; }

    /// <summary>Set once the tournament completes (see Phase 4) - null while still in progress.</summary>
    public int? FinalPlacement { get; set; }
}
