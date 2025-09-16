namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;

public interface IMapStatisticsService
{
    Task<IEnumerable<UiMatchLogDto>> GetMapMatchesAsync(Guid mapId);

    Task<IEnumerable<UiMapStatisticsDto>> GetMapsStatisticsAsync();

    Task<UiMapStatisticsDto> GetMapStatisticsAsync(Guid mapId);
}
