namespace Unmatched.Entities;

using System.ComponentModel.DataAnnotations;
using Unmatched.Enums;

public class Tournament
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TournamentType Type { get; set; }

    public bool IsActive { get; set; } = true;
    
    public Stage CurrentStage { get; set; }
    
    public virtual ICollection<Match> Matches { get; set; }
}