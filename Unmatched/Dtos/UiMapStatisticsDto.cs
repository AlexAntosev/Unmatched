namespace Unmatched.Dtos;

public class UiMapStatisticsDto
{
    public Guid MapId { get; set; }

    public string? Name { get; set; }

    public int TotalMatches { get; set; }

    public string ImageUrl => $"/{Name?.Replace(" ", string.Empty) ?? "Unknown"}.png";
}
