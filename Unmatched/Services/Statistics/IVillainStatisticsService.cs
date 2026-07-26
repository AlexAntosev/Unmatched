namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;

public interface IVillainStatisticsService
{
    Task<IEnumerable<VillainStatisticsDto>> GetVillainsStatisticsAsync();

    Task<VillainStatisticsDto> GetVillainStatisticsAsync(Guid villainId);

    Task<string> UpdateImageAsync(Guid villainId, string imageFileName);
}
