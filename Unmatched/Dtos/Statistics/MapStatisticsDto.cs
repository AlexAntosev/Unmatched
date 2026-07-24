namespace Unmatched.Dtos.Statistics;

public class MapStatisticsDto
{
    public Guid MapId { get; set; }

    public string Name { get; set; }

    public string? ImageFileName { get; set; }

    public int TotalMatches { get; set; }
}