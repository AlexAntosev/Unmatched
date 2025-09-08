namespace Unmatched.StatisticsService.Domain.Services;

public interface IVillainStatisticsService
{
    Task<IEnumerable<VillainStatisticsDto>> GetVillainsStatisticsAsync();
    
    Task<VillainStatisticsDto> GetVillainStatisticsAsync(Guid villainId);
}
