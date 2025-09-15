namespace Unmatched.StatisticsService.Api.Dto;

public class PlayerStatsDto
{
    public double Kd { get; set; }

    public int LastMatchPoints { get; set; }

    public string Name { get; set; }

    public int Place { get; set; }

    public Guid PlayerId { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }
}
