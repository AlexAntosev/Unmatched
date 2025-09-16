namespace Unmatched.StatisticsService.EntityFramework.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PlayerStats")]
public class PlayerStatsEntity
{
    public int LastMatchPoints { get; set; }

    public string Name { get; set; }

    public int Place { get; set; }

    [Key]
    public Guid PlayerId { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }
}
