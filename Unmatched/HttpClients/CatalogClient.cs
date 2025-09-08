namespace Unmatched.HttpClients;

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Unmatched.Dtos;
using Unmatched.Dtos.Catalog;
using Unmatched.HttpClients.Contracts;

public class CatalogClient(HttpClient httpClient) : ICatalogClient
{
    public async Task<CatalogHeroDto> GetHeroAsync(Guid id)
    {
        var response = await httpClient.GetAsync($"/hero/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CatalogHeroDto>();
    }

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

    public async Task<Guid> UpdatePlayStyleAsync(PlayStyleDto playStyle)
    {
        var content = new StringContent(JsonSerializer.Serialize(playStyle), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/hero/{playStyle.HeroId}/playstyle", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}