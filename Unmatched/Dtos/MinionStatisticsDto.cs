namespace Unmatched.Dtos;

using System;

public class MinionStatisticsDto : IComparable<MinionStatisticsDto>
{
    public Guid MinionId { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public int Hp { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string? ImageFileName { get; set; }

    public string ImageUrl => ImageFileName != null ? $"/images/minions/{ImageFileName}" : $"/{Name}.png";

    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";

    public double Kd { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public int CompareTo(MinionStatisticsDto? other)
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
