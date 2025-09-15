namespace Unmatched.StatisticsService.Domain.Services.Contracts;

using System;

using Unmatched.StatisticsService.Domain.Models;

public interface IPlayerStatisticsService
{
    Task<IEnumerable<PlayerStats>> GetPlayersStatisticsAsync();

    Task<PlayerStats> GetPlayerStatisticsAsync(Guid playerId);

}
