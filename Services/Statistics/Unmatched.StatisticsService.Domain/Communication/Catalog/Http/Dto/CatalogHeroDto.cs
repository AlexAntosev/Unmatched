namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

using System;

public class CatalogHeroDto
{
    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }

    public bool IsRanged { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public IEnumerable<CatalogSidekickDto> Sidekicks { get; set; }

    public string? ImageFileName { get; set; }

    /// <summary>Which box the hero ships in - used to roll hero stats up per expansion.</summary>
    public Guid? ExpansionId { get; set; }
}
