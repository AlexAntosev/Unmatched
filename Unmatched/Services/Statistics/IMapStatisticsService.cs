namespace Unmatched.Services.Statistics;

using System;

using Unmatched.Dtos;

public interface IMapStatisticsService
{
    Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync();

    Task<MapStatisticsDto> GetMapStatisticsAsync(Guid mapId);

    Task<IEnumerable<MatchLogDto>> GetMapMatchesAsync(Guid mapId);
}
