namespace Unmatched.MatchService.Domain.Dto.Catalog;

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
}
