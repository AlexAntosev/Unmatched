namespace Unmatched.StatisticsService.Api.Dto;

public class VillainStatsDto
{
    public Guid VillainId { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public int Hp { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string? ImageFileName { get; set; }

    public double Kd { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }
}
