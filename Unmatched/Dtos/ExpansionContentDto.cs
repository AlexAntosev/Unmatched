namespace Unmatched.Dtos;

/// <summary>
/// A hero, villain or minion that ships in an expansion, in the shape the collection screen needs:
/// a token, a deck size, a melee/ranged flag and HP.
/// </summary>
public class ExpansionContentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Hp { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string? ImageFileName { get; set; }

    /// <summary>Which MinIO category the art lives in - heroes, villains or minions.</summary>
    public string ImageCategory { get; set; } = "heroes";

    public string ImageUrl => ImageFileName != null ? $"/images/{ImageCategory}/{ImageFileName}" : "/Unknown.png";

    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";
}
