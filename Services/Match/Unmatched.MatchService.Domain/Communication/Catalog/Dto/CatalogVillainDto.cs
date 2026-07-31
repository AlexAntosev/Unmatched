namespace Unmatched.MatchService.Domain.Communication.Catalog.Dto;

using System;

public class CatalogVillainDto
{
    public int DeckSize { get; set; }

    public int BaseHp { get; set; }

    public int HpPerExtraPlayer { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }
}
