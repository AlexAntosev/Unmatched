namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Unmatched.MatchService.Domain.Enums;

/// <summary>One completion-bonus payout, written when a tournament is completed. Points is net of the
/// self-funded entry fee (see <see cref="Tournaments.TournamentAwardScheduler"/>), so it may be negative -
/// an early exit costs more than it earned. A hero can hold more than one row for the same tournament
/// (e.g. GroupStage's flat <see cref="TournamentAwardKind.GroupWinner"/> bonus alongside their playoff
/// tier), so uniqueness is on (TournamentId, HeroId, AwardKind), not just (TournamentId, HeroId).</summary>
[Table("TournamentAwards")]
public class TournamentAwardEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Guid HeroId { get; set; }

    public TournamentAwardKind AwardKind { get; set; }

    public int Points { get; set; }

    public DateTime AwardedAt { get; set; }
}
