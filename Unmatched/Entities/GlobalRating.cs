using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class GlobalRating
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid TournamentId { get; set; }
    
    public virtual Tournament Tournament { get; set; }
    
    public Guid HeroId { get; set; }
    
    public virtual Hero Hero { get; set; }
    
    public int Rating { get; set; }
}