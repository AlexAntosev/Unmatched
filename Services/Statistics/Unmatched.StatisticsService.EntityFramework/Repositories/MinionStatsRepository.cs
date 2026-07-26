namespace Unmatched.StatisticsService.EntityFramework.Repositories;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.EntityFramework.Context;
using Unmatched.StatisticsService.EntityFramework.Entities;

using MinionStats = Unmatched.StatisticsService.Domain.Models.MinionStats;

public class MinionStatsRepository(UnmatchedDbContext dbContext, IMapper mapper) : BaseRepository<MinionStats, MinionStatsEntity, UnmatchedDbContext>(dbContext, mapper),
                                                                                    IMinionStatsRepository
{
    protected override Guid GetId(MinionStatsEntity model)
    {
        return model.MinionId;
    }

    public async Task<MinionStats?> GetByMinionAsync(Guid minionId)
    {
        var entity = await DbContext.Set<MinionStatsEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.MinionId == minionId);
        return entity is null ? null : MapToModel(entity);
    }
}
