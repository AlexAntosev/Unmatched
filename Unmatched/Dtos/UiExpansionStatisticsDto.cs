namespace Unmatched.Dtos;

/// <summary>The three mini stats the Collection side panel shows for the selected set.</summary>
public class UiExpansionStatisticsDto
{
    public Guid ExpansionId { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public int TotalLooses { get; set; }

    public int WinRate { get; set; }

    public Guid? BestHeroId { get; set; }

    public string? BestHeroName { get; set; }
}
