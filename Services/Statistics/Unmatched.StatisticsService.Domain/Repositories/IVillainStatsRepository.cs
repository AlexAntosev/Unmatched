namespace Unmatched.StatisticsService.Domain.Repositories;

using System;

using Unmatched.StatisticsService.Domain.Models;

public interface IVillainStatsRepository : IRepository<VillainStats>
{
    Task<VillainStats?> GetByVillainAsync(Guid villainId);
}
