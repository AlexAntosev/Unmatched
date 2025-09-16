namespace Unmatched.StatisticsService.Domain.Repositories;

using System;

public interface IUnitOfWork : IDisposable
{
    IHeroStatsRepository HeroStats { get; }

    Task SaveChangesAsync();
}