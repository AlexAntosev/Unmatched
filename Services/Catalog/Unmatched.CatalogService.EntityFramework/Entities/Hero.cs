namespace Unmatched.CatalogService.EntityFramework.Entities;

using System.ComponentModel.DataAnnotations;

public class Hero
{
    public int DeckSize { get; set; }

    public int Hp { get; set; }

    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool IsRanged { get; set; }

    public string Name { get; set; }
    
    public virtual ICollection<Sidekick> Sidekicks { get; set; }
    
    public string Color { get; set; }
    
    public virtual PlayStyle? PlayStyle { get; set; }
}
