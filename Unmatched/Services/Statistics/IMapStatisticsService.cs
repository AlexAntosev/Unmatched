namespace Unmatched.Services.Statistics;

using System;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public interface IMapStatisticsService
{
    Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync();

    Task<MapStatisticsDto> GetMapStatisticsAsync(Guid mapId);

    Task<IEnumerable<MatchLogDto>> GetMapMatchesAsync(Guid mapId);
}
