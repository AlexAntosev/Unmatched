using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class DuelMatch
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid LeagueId { get; set; }
    
    public virtual League League { get; set; }
    
    public Guid MapId { get; set; }
    
    public virtual Map Map { get; set; }
    
    public Guid WinnerId { get; set; }
    
    public virtual Opponent Winner { get; set; }
    
    public Guid LoserId { get; set; }
    
    public virtual Opponent Loser { get; set; }
    
    public DateTime Date { get; set; }
    
    public string Comment { get; set; }
}