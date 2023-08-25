namespace Unmatched.Dtos;

using System;

public class MapStatisticsDto
{
    public Guid MapId { get; set; }
    
    public string MapName { get; set; }

    public int TotalMatches { get; set; }
}
