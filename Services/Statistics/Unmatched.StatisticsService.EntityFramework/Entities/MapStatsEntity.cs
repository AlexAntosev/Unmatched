namespace Unmatched.StatisticsService.EntityFramework.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("MapStats")]
public class MapStatsEntity
{
    [Key]
    public Guid MapId { get; set; }

    public string Name { get; set; }

    public int TotalMatches { get; set; }

    public DateTime ModifiedAt { get; set; }

    public DateTime LastMatchIncludedAt { get; set; }
}
