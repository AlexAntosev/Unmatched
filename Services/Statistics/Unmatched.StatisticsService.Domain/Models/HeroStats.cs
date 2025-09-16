namespace Unmatched.StatisticsService.Domain.Models;

public class HeroStats
{
    public string Color { get; set; }

    public int DeckSize { get; set; }

    public Guid HeroId { get; set; }

    public int Hp { get; set; }

    public bool IsRanged { get; set; }

    public double Kd
        => TotalMatches > 0
            ? Math.Round((double)TotalWins / TotalMatches, 2)
            : 0;

    public int LastMatchPoints { get; set; }

    public string Name { get; set; }

    public int Place { get; set; }

    public int Points { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public DateTime ModifiedAt { get; set; }

    public DateTime LastMatchIncludedAt { get; set; }
}
