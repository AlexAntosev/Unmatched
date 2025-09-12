namespace Unmatched.StatisticsService.Domain.Services;

using System;

using Unmatched.StatisticsService.Domain.Models;

public interface IPlayerStatisticsService
{
    Task<IEnumerable<PlayerStats>> GetPlayersStatisticsAsync();

    Task<PlayerStats> GetPlayerStatisticsAsync(Guid playerId);

    Task<IEnumerable<MatchLogDto>> GetPlayerMatchesAsync(Guid playerId); // match
}
