namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Unmatched.MatchService.Domain.Enums;

[Table("Tournament")]
public class TournamentEntity
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TournamentType Type { get; set; }

    public bool IsActive { get; set; } = true;
    
    public Stage InitialStage { get; set; }
    
    public Stage CurrentStage { get; set; }
    
    public virtual ICollection<MatchEntity> Matches { get; set; }
}