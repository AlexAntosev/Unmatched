namespace Unmatched.MatchService.Domain.Catalog;

using System.Net.Http.Json;

using Unmatched.MatchService.Domain.Dto.Catalog;

public class CatalogClient(HttpClient httpClient) : ICatalogClient
{
    public async Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync()
    {
        var response = await httpClient.GetAsync("/hero");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogHeroDto>>();
    }
}