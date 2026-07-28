namespace Unmatched.StatisticsService.Domain.Repositories;

using System;

using Unmatched.StatisticsService.Domain.Models;

public interface IMinionStatsRepository : IRepository<MinionStats>
{
    Task<MinionStats?> GetByMinionAsync(Guid minionId);
}
