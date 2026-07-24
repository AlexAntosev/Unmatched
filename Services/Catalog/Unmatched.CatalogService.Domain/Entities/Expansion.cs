namespace Unmatched.CatalogService.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Expansion
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public int? ReleaseYear { get; set; }

    public string? Publisher { get; set; }

    public virtual ICollection<Hero> Heroes { get; set; }

    public virtual ICollection<Map> Maps { get; set; }

    public virtual ICollection<Villain> Villains { get; set; }

    public virtual ICollection<Minion> Minions { get; set; }
}
