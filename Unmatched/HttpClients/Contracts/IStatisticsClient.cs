namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos.Statistics;

public interface IStatisticsClient
{
    Task<HeroStatisticsDto> GetHeroStatsAsync(Guid heroId);

    Task<IEnumerable<HeroStatisticsDto>> GetHeroStatsAsync();
}
