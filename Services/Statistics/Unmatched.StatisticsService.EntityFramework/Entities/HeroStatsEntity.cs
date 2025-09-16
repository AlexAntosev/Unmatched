namespace Unmatched.StatisticsService.EntityFramework.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("HeroStats")]
public class HeroStatsEntity
{
    public string Color { get; set; }

    public int DeckSize { get; set; }

    [Key]
    public Guid HeroId { get; set; }

    public int Hp { get; set; }

    public bool IsRanged { get; set; }

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
