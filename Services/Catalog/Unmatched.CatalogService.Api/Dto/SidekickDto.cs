namespace Unmatched.CatalogService.Api.Dto;

public class SidekickDto
{
    public int Count { get; set; } = 1;

    public Guid? HeroId { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }

    public bool IsRanged { get; set; }

    public string Name { get; set; }
}
