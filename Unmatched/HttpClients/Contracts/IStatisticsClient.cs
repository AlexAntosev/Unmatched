namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos;
using Unmatched.Dtos.Statistics;

public interface IStatisticsClient
{
    Task<HeroStatisticsDto> GetHeroStatsAsync(Guid heroId);

    Task<IEnumerable<HeroStatisticsDto>> GetHeroStatsAsync();

    Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync();

    Task<MapStatisticsDto> GetMapsStatisticsAsync(Guid mapId);

    Task<PlayerStatisticsDto> GetPlayerStatsAsync(Guid playerId);

    Task<IEnumerable<PlayerStatisticsDto>> GetPlayerStatsAsync();

    Task<VillainStatisticsDto> GetVillainStatsAsync(Guid villainId);

    Task<IEnumerable<VillainStatisticsDto>> GetVillainStatsAsync();

    Task<MinionStatisticsDto> GetMinionStatsAsync(Guid minionId);

    Task<IEnumerable<MinionStatisticsDto>> GetMinionStatsAsync();
}
