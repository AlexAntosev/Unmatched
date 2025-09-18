namespace Unmatched.StatisticsService.EntityFramework.Repositories;

using AutoMapper;

using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.EntityFramework.Context;
using Unmatched.StatisticsService.EntityFramework.Entities;

public class MapStatsRepository(UnmatchedDbContext dbContext, IMapper mapper) : BaseRepository<MapStats, MapStatsEntity, UnmatchedDbContext>(dbContext, mapper), IMapStatsRepository
{
    protected override Guid GetId(MapStatsEntity model)
    {
        return model.MapId;
    }
}
