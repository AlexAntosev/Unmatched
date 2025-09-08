namespace Unmatched.StatisticsService.Domain.Services;

using System;

public interface IMapStatisticsService
{
    Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync();

    Task<MapStatisticsDto> GetMapStatisticsAsync(Guid mapId);

    Task<IEnumerable<MatchLogDto>> GetMapMatchesAsync(Guid mapId);
}
