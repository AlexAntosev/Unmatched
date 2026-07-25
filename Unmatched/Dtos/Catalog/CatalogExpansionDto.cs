namespace Unmatched.Dtos.Catalog;

public class CatalogExpansionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int? ReleaseYear { get; set; }

    public string? Publisher { get; set; }

    public IEnumerable<string> HeroNames { get; set; }

    public IEnumerable<CatalogMapDto> Maps { get; set; }
}
