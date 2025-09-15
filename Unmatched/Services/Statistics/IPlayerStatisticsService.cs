namespace Unmatched.Services.Statistics;

using System;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;
using Unmatched.HttpClients;

public interface IPlayerStatisticsService
{
    Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync();

    Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId);

    Task<IEnumerable<UiMatchLogDto>> GetPlayerMatchesAsync(Guid playerId);
}