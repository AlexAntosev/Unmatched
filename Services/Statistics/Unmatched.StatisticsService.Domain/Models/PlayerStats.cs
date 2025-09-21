namespace Unmatched.StatisticsService.Domain.Models;

public class PlayerStats
{
    public double Kd
        => TotalMatches > 0
            ? Math.Round((double)TotalWins / TotalLooses, 2)
            : 0;

    public int LastMatchPoints { get; set; }

    public string Name { get; set; }

    public int Place { get; set; }

    public Guid PlayerId { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }
}
