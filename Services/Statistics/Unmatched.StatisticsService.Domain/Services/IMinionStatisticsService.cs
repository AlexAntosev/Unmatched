namespace Unmatched.StatisticsService.Domain.Services;

using Unmatched.StatisticsService.Domain.Models;

public interface IMinionStatisticsService
{
    Task<IEnumerable<MinionStats>> GetMinionsStatisticsAsync();
    
    Task<MinionStats> GetMinionStatisticsAsync(Guid minionId);
}
