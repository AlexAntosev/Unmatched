using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class Hero
{
    public Hero()
    {
        Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Hp { get; set; }
    
    public int DeckSize { get; set; }
    
    public IEnumerable<Sidekick> Sidekicks { get; set; }
    
    public bool IsRanged { get; set; }
}