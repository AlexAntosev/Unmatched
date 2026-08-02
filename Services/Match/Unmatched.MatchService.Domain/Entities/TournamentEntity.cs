namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Unmatched.MatchService.Domain.Enums;

[Table("Tournaments")]
public class TournamentEntity
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TournamentFormat Format { get; set; }

    public TournamentStatus Status { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int MaxParticipants { get; set; }

    public string? ImageFileName { get; set; }

    public string? TrophyImageFileName { get; set; }

    /// <summary>Meaningful for SingleElimination (and the playoff phase a GroupStage tournament moves
    /// into once its groups finish) - the bracket round. Swiss and League track progress by
    /// <see cref="MatchEntity.Round"/> / match count instead, since they have no elimination rounds.</summary>
    public Stage InitialStage { get; set; }

    public Stage CurrentStage { get; set; }

    public virtual ICollection<MatchEntity> Matches { get; set; }

    public virtual ICollection<TournamentParticipantEntity> Participants { get; set; } = new List<TournamentParticipantEntity>();

    public virtual ICollection<TournamentTitleEntity> TournamentTitles { get; set; } = new List<TournamentTitleEntity>();
}
