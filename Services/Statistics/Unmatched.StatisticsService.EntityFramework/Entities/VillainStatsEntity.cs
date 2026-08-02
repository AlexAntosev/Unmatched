namespace Unmatched.StatisticsService.EntityFramework.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("VillainStats")]
public class VillainStatsEntity
{
    [Key]
    public Guid VillainId { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }

    public int BaseHp { get; set; }

    public int HpPerExtraPlayer { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }

    public string? ImageFileName { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public DateTime ModifiedAt { get; set; }

    public DateTime LastMatchIncludedAt { get; set; }
}
