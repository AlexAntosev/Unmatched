namespace Unmatched.CatalogService.Api.Dto;

public class ExpansionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int? ReleaseYear { get; set; }

    public string? Publisher { get; set; }

    public string? ImageFileName { get; set; }

    public IEnumerable<ExpansionContentDto> Heroes { get; set; } = [];

    public IEnumerable<MapDto> Maps { get; set; } = [];

    public IEnumerable<ExpansionContentDto> Villains { get; set; } = [];

    public IEnumerable<ExpansionContentDto> Minions { get; set; } = [];
}
