namespace Unmatched.Dtos;

using System;

public class VillainStatisticsDto : IComparable<VillainStatisticsDto>
{
    public Guid VillainId { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public int BaseHp { get; set; }

    public int HpPerExtraPlayer { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string? ImageFileName { get; set; }

    public string ImageUrl => ImageFileName != null ? $"/images/villains/{ImageFileName}" : "/Unknown.png";

    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";

    public double Kd { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public int CompareTo(VillainStatisticsDto? other)
    {
        if (other == null)
        {
            return 0;
        }

        if (Kd != other.Kd)
        {
            return Kd > other.Kd
                ? 1
                : -1;
        }

        if (TotalMatches != other.TotalMatches)
        {
            return TotalMatches > other.TotalMatches
                ? 1
                : -1;
        }

        return Name.CompareTo(other.Name);
    }
}
