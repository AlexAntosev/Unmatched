namespace Unmatched.Dtos;

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

    /// <summary>Null when no cover has been uploaded - the shelf then draws the dashed placeholder.</summary>
    public string? ImageUrl => ImageFileName != null ? $"/images/expansions/{ImageFileName}" : null;

    public int HeroCount => Heroes.Count();

    public int MapCount => Maps.Count();

    public int VillainCount => Villains.Count();

    public int MinionCount => Minions.Count();
}
