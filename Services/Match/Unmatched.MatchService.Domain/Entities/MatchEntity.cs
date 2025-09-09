namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Unmatched.MatchService.Domain.Enums;

[Table("Match")]
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
    
    public bool IsPlanned { get; set; }
    
    public Stage? Stage { get; set; }
    
    public int? Epic { get; set; }
}
