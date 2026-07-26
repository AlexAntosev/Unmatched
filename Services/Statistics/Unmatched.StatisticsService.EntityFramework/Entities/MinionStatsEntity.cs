namespace Unmatched.StatisticsService.EntityFramework.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("MinionStats")]
public class MinionStatsEntity
{
    [Key]
    public Guid MinionId { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public int Hp { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string? ImageFileName { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public DateTime ModifiedAt { get; set; }

    public DateTime LastMatchIncludedAt { get; set; }
}
