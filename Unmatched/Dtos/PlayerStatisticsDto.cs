namespace Unmatched.Dtos;

using System;
using Unmatched.Dtos.Player;

public class PlayerStatisticsDto : IComparable<PlayerStatisticsDto>
{
    public UiPlayerDto? Player { get; set; }
    public Guid PlayerId { get; set; }

    public double Kd
        => TotalMatches > 0
            ? Math.Round((double)TotalWins / TotalMatches, 2)
            : 0;

    public int LastMatchPoints { get; set; }

    public int Place { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public int CompareTo(PlayerStatisticsDto? other)
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

        return Player.Name.CompareTo(other.Player.Name);

    }
}