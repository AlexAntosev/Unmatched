namespace Unmatched.Dtos;

using System;

public class MapStatisticsDto
{
    public MapDto? Map { get; set; }
    
    public Guid MapId { get; set; }

    public int TotalMatches { get; set; }
}
