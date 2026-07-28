namespace Unmatched.CatalogService.Api.Dto;

/// <summary>
/// A fighter that ships in a box. Heroes, villains and minions carry the same handful of facts the
/// collection screen shows (deck size, melee/ranged, HP), so they share one shape.
/// </summary>
public class ExpansionContentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Hp { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string? ImageFileName { get; set; }
}
