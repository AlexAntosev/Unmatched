using Unmatched.HttpClients.Contracts;

namespace Unmatched.HttpClients;

using System.Net.Http;
using System.Net.Http.Json;

using Unmatched.Dtos.Statistics;

public class StatisticsClient(HttpClient httpClient) : IStatisticsClient
{
    public async Task<HeroStatisticsDto> GetHeroStatsAsync(Guid heroId)
    {
        var response = await httpClient.GetAsync($"/hero/{heroId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<HeroStatisticsDto>();
    }

    public async Task<IEnumerable<HeroStatisticsDto>> GetHeroStatsAsync()
    {
        var response = await httpClient.GetAsync("/hero");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<HeroStatisticsDto>>();
    }
}
