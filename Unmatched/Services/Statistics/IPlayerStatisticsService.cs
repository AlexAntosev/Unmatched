namespace Unmatched.Services.Statistics;

using System;

using Unmatched.Dtos;

public interface IPlayerStatisticsService
{
    Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync();

    Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId);

    Task<IEnumerable<MatchLogDto>> GetPlayerMatchesAsync(Guid playerId);
}
