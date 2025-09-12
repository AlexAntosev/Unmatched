namespace Unmatched.StatisticsService.Domain.Models;

using System;

public class FavoriteHeroStats : IComparable<FavoriteHeroStats>
{
    public Guid FavoriteId { get; set; }
    
    public Guid HeroId { get; set; }

    public UiHeroDto? Hero { get; set; }
    
    public Guid PlayerId { get; set; }

    public PlayerDto? Player { get; set; }
    
    public List<FighterDto> Fights { get; set; }

    public bool IsChosenOne { get; set; }
    
    public int Favour { get; set; }

    public double Kd
        => TotalMatches > 0
            ? Math.Round((double)TotalWins / TotalMatches, 2)
            : 0;

    public int Place { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public int CompareTo(FavoriteHeroStats? other)
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

        return Hero.Name.CompareTo(other.Hero.Name);
    }
}
