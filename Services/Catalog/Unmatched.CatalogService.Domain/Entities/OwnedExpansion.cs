namespace Unmatched.CatalogService.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class OwnedExpansion
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ExpansionId { get; set; }

    public virtual Expansion Expansion { get; set; }
}
