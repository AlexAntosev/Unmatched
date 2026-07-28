namespace Unmatched.Dtos.Catalog;

/// <summary>Mirrors the catalog service's ExpansionContentDto - a hero, villain or minion in a box.</summary>
public class CatalogExpansionContentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Hp { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string? ImageFileName { get; set; }
}
