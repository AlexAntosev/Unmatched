namespace Unmatched.Data.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Unmatched.Data.Enums;

public class Match 
{
    public string? Comment { get; set; }

    public DateTime Date { get; set; }

    [Key]
    public Guid Id { get; set; }

    public virtual Map? Map { get; set; }

    public Guid? MapId { get; set; }
    
    [ForeignKey(nameof(Fighter.MatchId))]
    public virtual ICollection<Fighter> Fighters { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public Guid? TournamentId { get; set; }
    
    public bool IsPlanned { get; set; }
    
    public Stage? Stage { get; set; }
}
