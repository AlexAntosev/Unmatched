using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class Rating
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid HeroId { get; set; }
    
    public virtual Hero Hero { get; set; }
    
    public int Points { get; set; }
}