namespace Unmatched.StatisticsService.Api.Dto;

using System;

public class MapStatsDto
{
    public Guid MapId { get; set; }

    public string Name { get; set; }

    public int TotalMatches { get; set; }
}
