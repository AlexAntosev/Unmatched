namespace Unmatched.Dtos.Catalog;

public class CatalogPlayStyleDto
{
    public Guid Id { get; set; }

    public Guid HeroId { get; set; }

    public int Attack { get; set; }

    public int Defence { get; set; }

    public int Trickery { get; set; }

    public int Difficulty { get; set; }
}
