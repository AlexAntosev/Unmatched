using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class Sidekick
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Hp { get; set; }
    
    public Guid HeroId { get; set; }
    
    public virtual Hero Hero { get; set; }
    
    public bool IsRanged { get; set; }

    public int Count { get; set; } = 1;
}