namespace Unmatched.StatisticsService.Domain.Services.Contracts;

using Unmatched.StatisticsService.Domain.Models;

public interface IVillainStatisticsService
{
    Task<IEnumerable<VillainStats>> GetVillainsStatisticsAsync();
    
    Task<VillainStats> GetVillainStatisticsAsync(Guid villainId);
}
