namespace Unmatched.StatisticsService.Domain.Services;

using System;

public interface IPlayerStatisticsService
{
    Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync();

    Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId);

    Task<IEnumerable<MatchLogDto>> GetPlayerMatchesAsync(Guid playerId);
}
