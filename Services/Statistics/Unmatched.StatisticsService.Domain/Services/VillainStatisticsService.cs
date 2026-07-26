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

    public async Task<VillainStats?> UpdateImageAsync(Guid villainId, string imageFileName)
    {
        var stats = await unitOfWork.VillainStats.GetByVillainAsync(villainId);
        if (stats is null)
        {
            return null;
        }

        stats.ImageFileName = imageFileName;
        await unitOfWork.VillainStats.AddOrUpdateAsync(stats);
        await unitOfWork.SaveChangesAsync();
        return stats;
    }
}
