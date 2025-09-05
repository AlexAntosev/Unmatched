namespace Unmatched.CatalogService.EntityFramework.Entities;

using System.ComponentModel.DataAnnotations;

public class Sidekick
{
    public Sidekick()
    {
        Id = Guid.NewGuid();
    }

    public int Count { get; set; } = 1;

    public virtual Hero? Hero { get; set; }

    public Guid? HeroId { get; set; }

    public int Hp { get; set; }

    [Key]
    public Guid Id { get; set; }

    public bool IsRanged { get; set; }

    public string Name { get; set; }
}
