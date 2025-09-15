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

    public async Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync()
    {
        var response = await httpClient.GetAsync("/map");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<MapStatisticsDto>>();
    }

    public async Task<MapStatisticsDto> GetMapsStatisticsAsync(Guid mapId)
    {
        var response = await httpClient.GetAsync($"/map/{mapId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MapStatisticsDto>();
    }
}
