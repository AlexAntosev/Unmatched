namespace Unmatched.StatisticsService.Domain.Services;

using Unmatched.StatisticsService.Domain.Models;

public interface IVillainStatisticsService
{
    Task<IEnumerable<VillainStats>> GetVillainsStatisticsAsync();
    
    Task<VillainStats> GetVillainStatisticsAsync(Guid villainId);
}
