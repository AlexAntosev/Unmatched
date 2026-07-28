namespace Unmatched.StatisticsService.Domain.Services;

using AutoMapper;

using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.Domain.Services.Contracts;

public class MapStatisticsService(IMapper mapper, IUnitOfWork unitOfWork) : IMapStatisticsService
{
    public async Task<IEnumerable<MapStats>> GetMapsStatisticsAsync()
    {
        var maps = await unitOfWork.MapStats.GetAllAsync();

        return maps;
    }

    public async Task<MapStats> GetMapStatisticsAsync(Guid mapId)
    {
        var map = await unitOfWork.MapStats.GetAsync(mapId);
        return map;
    }

    public async Task<MapStats?> UpdateImageAsync(Guid mapId, string imageFileName)
    {
        var stats = await unitOfWork.MapStats.GetAsync(mapId);
        if (stats is null)
        {
            return null;
        }

        stats.ImageFileName = imageFileName;
        await unitOfWork.MapStats.AddOrUpdateAsync(stats);
        await unitOfWork.SaveChangesAsync();
        return stats;
    }
}
