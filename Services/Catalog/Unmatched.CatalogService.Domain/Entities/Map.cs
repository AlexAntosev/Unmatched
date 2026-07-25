namespace Unmatched.CatalogService.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Map
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? OfficialName { get; set; }

    public Guid? ExpansionId { get; set; }

    public virtual Expansion? Expansion { get; set; }

    public string? ImageFileName { get; set; }
}
