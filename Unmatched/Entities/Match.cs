using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class Match
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid? TournamentId { get; set; }
    
    public virtual Tournament? Tournament { get; set; }
    
    public Guid MapId { get; set; }
    
    public virtual Map Map { get; set; }
    
    public DateTime Date { get; set; }
    
    public string Comment { get; set; }
}