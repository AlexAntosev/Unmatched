namespace Unmatched.Services.Statistics;

using System;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public interface IPlayerStatisticsService
{
    Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync();

    Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId);

    Task<IEnumerable<MatchLogDto>> GetPlayerMatchesAsync(Guid playerId);
}
