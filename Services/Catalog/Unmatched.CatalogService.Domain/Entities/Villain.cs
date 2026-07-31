namespace Unmatched.CatalogService.Domain.Entities;

using System;
using System.ComponentModel.DataAnnotations;

public class Villain
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; }

    /// <summary>HP for a single hero player. Co-op HP scales with the number of players facing the
    /// villain - see <see cref="HpPerExtraPlayer"/> - and this is that scale's base case, not a flat
    /// total. "10 HP per player" villains simply have <c>HpPerExtraPlayer == BaseHp</c>.</summary>
    public int BaseHp { get; set; }

    /// <summary>HP added per player beyond the first. Not every villain scales at the same rate per
    /// player (e.g. Shredder/Krang add less per player than their base), so this is tracked
    /// separately from <see cref="BaseHp"/> rather than assumed equal to it.</summary>
    public int HpPerExtraPlayer { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string Color { get; set; }

    public Guid? ExpansionId { get; set; }

    public virtual Expansion? Expansion { get; set; }

    public string? ImageFileName { get; set; }
}
