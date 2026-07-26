namespace Unmatched.StatisticsService.EntityFramework.Repositories;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.EntityFramework.Context;
using Unmatched.StatisticsService.EntityFramework.Entities;

using VillainStats = Unmatched.StatisticsService.Domain.Models.VillainStats;

public class VillainStatsRepository(UnmatchedDbContext dbContext, IMapper mapper) : BaseRepository<VillainStats, VillainStatsEntity, UnmatchedDbContext>(dbContext, mapper),
                                                                                     IVillainStatsRepository
{
    protected override Guid GetId(VillainStatsEntity model)
    {
        return model.VillainId;
    }

    public async Task<VillainStats?> GetByVillainAsync(Guid villainId)
    {
        var entity = await DbContext.Set<VillainStatsEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.VillainId == villainId);
        return entity is null ? null : MapToModel(entity);
    }
}
