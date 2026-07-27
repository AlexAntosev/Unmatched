namespace Unmatched.Dtos.Catalog;

public class CatalogExpansionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int? ReleaseYear { get; set; }

    public string? Publisher { get; set; }

    public string? ImageFileName { get; set; }

    public IEnumerable<CatalogExpansionContentDto> Heroes { get; set; } = [];

    public IEnumerable<CatalogMapDto> Maps { get; set; } = [];

    public IEnumerable<CatalogExpansionContentDto> Villains { get; set; } = [];

    public IEnumerable<CatalogExpansionContentDto> Minions { get; set; } = [];
}
