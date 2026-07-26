namespace Unmatched.StatisticsService.Domain.Services;

using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.Domain.Services.Contracts;

public class VillainStatisticsService(IUnitOfWork unitOfWork) : IVillainStatisticsService
{
    public async Task<IEnumerable<VillainStats>> GetVillainsStatisticsAsync()
    {
        return await unitOfWork.VillainStats.GetAllAsync();
    }

    public async Task<VillainStats?> GetVillainStatisticsAsync(Guid villainId)
    {
        return await unitOfWork.VillainStats.GetByVillainAsync(villainId);
    }
}
