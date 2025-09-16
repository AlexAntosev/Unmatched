namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

using System.Net.Http.Json;
using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

public class CatalogClient(HttpClient httpClient) : ICatalogClient
{
    public async Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync()
    {
        var response = await httpClient.GetAsync("/hero");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogHeroDto>>();
    }

    public async Task<IEnumerable<CatalogMapDto>> GetMapsAsync()
    {
        var response = await httpClient.GetAsync("/map");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogMapDto>>();
    }

    public async Task<IEnumerable<CatalogSidekickDto>> GetSidekicksAsync()
    {
        var response = await httpClient.GetAsync($"/sidekick");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogSidekickDto>>();
    }
}
