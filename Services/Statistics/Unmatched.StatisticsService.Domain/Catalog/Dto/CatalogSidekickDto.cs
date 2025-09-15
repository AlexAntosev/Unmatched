namespace Unmatched.StatisticsService.Domain.Catalog.Dto;

public class CatalogSidekickDto
{
    public int Count { get; set; }

    public Guid? HeroId { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }

    public bool IsRanged { get; set; }

    public string Name { get; set; }
}
