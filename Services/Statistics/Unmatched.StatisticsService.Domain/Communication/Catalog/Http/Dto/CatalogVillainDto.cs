namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

using System;

public class CatalogVillainDto
{
    public int DeckSize { get; set; }

    public int BaseHp { get; set; }

    public int HpPerExtraPlayer { get; set; }

    public Guid Id { get; set; }

    public bool IsRanged { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public string? ImageFileName { get; set; }
}
