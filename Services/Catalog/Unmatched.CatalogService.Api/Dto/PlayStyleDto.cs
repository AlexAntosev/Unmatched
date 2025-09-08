namespace Unmatched.CatalogService.Api.Dto;

public class PlayStyleDto
{
    public Guid Id { get; set; }

    public Guid HeroId { get; set; }

    public int Attack { get; set; }

    public int Defence { get; set; }

    public int Trickery { get; set; }

    public int Difficulty { get; set; }
}
