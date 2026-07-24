namespace Unmatched.StatisticsService.Domain.Services;

using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Communication.Player.Http;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Services.Contracts;

public class PlayerStatisticsService(IMatchClient matchClient, IPlayerCache playerCache) : IPlayerStatisticsService
{
    public async Task<IEnumerable<PlayerStats>> GetPlayersStatisticsAsync()
    {
        var players = await playerCache.GetAsync();
        var matches = (await matchClient.GetMatchLogAsync()).ToList();

        return players.Select(player => BuildStats(player.Id, player.Name, matches)).ToList();
    }

    public async Task<PlayerStats> GetPlayerStatisticsAsync(Guid playerId)
    {
        var player = await playerCache.GetAsync(playerId);
        var matches = await matchClient.GetFinishedByPlayerAsync(playerId);

        return BuildStats(playerId, player?.Name ?? string.Empty, matches);
    }

    private static PlayerStats BuildStats(Guid playerId, string name, IEnumerable<MatchLogDto> matches)
    {
        var playerFights = matches
            .OrderByDescending(match => match.Date)
            .Select(match => match.Fighters.FirstOrDefault(fighter => fighter.Player?.Id == playerId))
            .Where(fighter => fighter is not null)
            .Select(fighter => fighter!)
            .ToList();

        return new PlayerStats
            {
                PlayerId = playerId,
                Name = name,
                TotalMatches = playerFights.Count,
                TotalWins = playerFights.Count(fighter => fighter.IsWinner),
                TotalLooses = playerFights.Count(fighter => fighter.IsWinner == false),
                LastMatchPoints = playerFights.FirstOrDefault()?.MatchPoints ?? 0
            };
    }
}
