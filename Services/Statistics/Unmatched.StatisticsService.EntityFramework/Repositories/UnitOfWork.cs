namespace Unmatched.StatisticsService.EntityFramework.Repositories;

using AutoMapper;

using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.EntityFramework.Context;

public class UnitOfWork(UnmatchedDbContext context, IMapper mapper) : IUnitOfWork
{
    public IHeroStatsRepository HeroStats { get; } = new HeroStatsRepository(context, mapper);

    public IMapStatsRepository MapStats { get; } = new MapStatsRepository(context, mapper);

    public void Dispose() => context.Dispose();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
