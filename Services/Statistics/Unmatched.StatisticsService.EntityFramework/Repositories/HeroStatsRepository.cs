namespace Unmatched.StatisticsService.EntityFramework.Repositories;

using AutoMapper;

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
}
