namespace Unmatched.StatisticsService.Domain.Repositories;

using System;

using Unmatched.StatisticsService.Domain.Models;

public interface IHeroStatsRepository : IRepository<HeroStats>
{
    Task<HeroStats> GetByHeroAsync(Guid heroId);
}