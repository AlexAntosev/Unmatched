namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;

public interface IMinionStatisticsService
{
    Task<IEnumerable<MinionStatisticsDto>> GetMinionsStatisticsAsync();
    
    Task<MinionStatisticsDto> GetMinionStatisticsAsync(Guid minionId);
}
