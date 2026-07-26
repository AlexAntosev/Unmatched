namespace Unmatched.StatisticsService.Domain.Services;

using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.Domain.Services.Contracts;

public class MinionStatisticsService(IUnitOfWork unitOfWork) : IMinionStatisticsService
{
    public async Task<IEnumerable<MinionStats>> GetMinionsStatisticsAsync()
    {
        return await unitOfWork.MinionStats.GetAllAsync();
    }

    public async Task<MinionStats?> GetMinionStatisticsAsync(Guid minionId)
    {
        return await unitOfWork.MinionStats.GetByMinionAsync(minionId);
    }

    public async Task<MinionStats?> UpdateImageAsync(Guid minionId, string imageFileName)
    {
        var stats = await unitOfWork.MinionStats.GetByMinionAsync(minionId);
        if (stats is null)
        {
            return null;
        }

        stats.ImageFileName = imageFileName;
        await unitOfWork.MinionStats.AddOrUpdateAsync(stats);
        await unitOfWork.SaveChangesAsync();
        return stats;
    }
}
