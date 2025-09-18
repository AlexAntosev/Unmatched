namespace Unmatched.StatisticsService.EntityFramework.Repositories;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.EntityFramework.Context;
using Unmatched.StatisticsService.EntityFramework.Entities;

using HeroStats = Unmatched.StatisticsService.Domain.Models.HeroStats;

public class HeroStatsRepository(UnmatchedDbContext dbContext, IMapper mapper) : BaseRepository<HeroStats, HeroStatsEntity, UnmatchedDbContext>(dbContext, mapper),
                                                                                 IHeroStatsRepository
{
    protected override Guid GetId(HeroStatsEntity model)
    {
        return model.HeroId;
    }

    public async Task<HeroStats> GetByHeroAsync(Guid heroId)
    {
        var entity = await DbContext.HeroStats.FirstOrDefaultAsync(x => x.HeroId == heroId);
        var model = MapToModel(entity!);
        return model;
    }
}
