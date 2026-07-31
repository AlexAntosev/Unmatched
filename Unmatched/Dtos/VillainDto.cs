namespace Unmatched.Dtos;

using System;

public class VillainDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    /// <summary>HP for a single hero player - see <see cref="EffectiveHp"/> for the co-op total
    /// against a given number of players.</summary>
    public int BaseHp { get; set; }

    public int HpPerExtraPlayer { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }
    
    public string Color { get; set; }

    public string? ImageFileName { get; set; }

    public Guid? ExpansionId { get; set; }

    public string? ExpansionName { get; set; }

    public string ImageUrl => ImageFileName != null ? $"/images/villains/{ImageFileName}" : "/Unknown.png";

    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";

    /// <summary>Total co-op HP against <paramref name="playerCount"/> heroes: the base case for the
    /// first player, plus <see cref="HpPerExtraPlayer"/> for each one beyond that.</summary>
    public int EffectiveHp(int playerCount) => BaseHp + HpPerExtraPlayer * Math.Max(0, playerCount - 1);
}
