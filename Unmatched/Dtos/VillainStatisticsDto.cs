﻿namespace Unmatched.Dtos;

using System;

public class VillainStatisticsDto : IComparable<VillainStatisticsDto>
{
    public VillainDto? Villain { get; set; }
    
    public Guid VillainId { get; set; }

    public double Kd
        => TotalMatches > 0
            ? Math.Round((double)TotalWins / TotalMatches, 2)
            : 0;

    public int LastMatchPoints { get; set; }
    
    public int Points { get; set; }

    public int Place { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }
    
    public int CompareTo(VillainStatisticsDto? other)
    {
        if (other == null)
        {
            return 0;
        }
        
        if (Points != other.Points)
        {
            return Points > other.Points
                ? 1
                : -1;
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

        return Villain.Name.CompareTo(other.Villain.Name);
    }
}
