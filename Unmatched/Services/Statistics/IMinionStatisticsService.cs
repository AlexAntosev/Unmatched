namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public interface IMinionStatisticsService
{
    Task<IEnumerable<MinionStatisticsDto>> GetMinionsStatisticsAsync();

    Task<MinionStatisticsDto> GetMinionStatisticsAsync(Guid minionId);

    Task<string> UpdateImageAsync(Guid minionId, string imageFileName);

    Task<IEnumerable<UiMatchLogDto>> GetMinionMatchesAsync(Guid minionId);
}
