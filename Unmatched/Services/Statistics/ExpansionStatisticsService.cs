namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class ExpansionStatisticsService(IMapper mapper, IStatisticsClient statisticsClient) : IExpansionStatisticsService
{
    /// <summary>Keyed by expansion so the Collection screen can look a set up without scanning.</summary>
    public async Task<IReadOnlyDictionary<Guid, UiExpansionStatisticsDto>> GetByExpansionAsync()
    {
        var stats = await statisticsClient.GetExpansionStatsAsync();
        return stats
            .Select(mapper.Map<UiExpansionStatisticsDto>)
            .ToDictionary(s => s.ExpansionId);
    }
}
