namespace Unmatched.CatalogService.Api.Dto;

public class VillainDto
{
    public string Color { get; set; }

    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }

    public bool IsRanged { get; set; }

    public string Name { get; set; }
}
