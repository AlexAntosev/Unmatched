namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Unmatched.MatchService.Domain.Enums;

[Table("Matches")]
public class MatchEntity 
{
    public string? Comment { get; set; }

    public DateTime Date { get; set; }

    [Key]
    public Guid Id { get; set; }

    public Guid? MapId { get; set; }
    
    [ForeignKey(nameof(FighterEntity.MatchId))]
    public virtual ICollection<FighterEntity> Fighters { get; set; }

    public virtual TournamentEntity? Tournament { get; set; }

    public Guid? TournamentId { get; set; }

    /// <summary>Whether this match affects hero rating. Independent of <see cref="TournamentId"/> -
    /// a match can belong to a tournament and still be Unranked (e.g. its HP/cards weren't recorded),
    /// and a standalone match can be Ranked.</summary>
    public bool IsRanked { get; set; }

    public bool IsPlanned { get; set; }

    public Stage? Stage { get; set; }

    /// <summary>Which Swiss round or group-stage round this match belongs to. Only meaningful for
    /// those two formats - SingleElimination uses <see cref="Stage"/> instead, and League/Bounty
    /// matches don't have rounds at all.</summary>
    public int? Round { get; set; }

    public int? Epic { get; set; }

    public GameMode GameMode { get; set; }

    public virtual MatchVillainEntity? Villain { get; set; }
}
