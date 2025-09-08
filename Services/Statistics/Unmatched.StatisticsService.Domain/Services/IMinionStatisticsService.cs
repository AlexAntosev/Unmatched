namespace Unmatched.StatisticsService.Domain.Services;

public interface IMinionStatisticsService
{
    Task<IEnumerable<MinionStatisticsDto>> GetMinionsStatisticsAsync();
    
    Task<MinionStatisticsDto> GetMinionStatisticsAsync(Guid minionId);
}
