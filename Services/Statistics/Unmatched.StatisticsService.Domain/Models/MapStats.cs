namespace Unmatched.StatisticsService.Domain.Models;

using System;

public class MapStats
{
    public Guid MapId { get; set; }

    public string Name { get; set; }

    public int TotalMatches { get; set; }
}
