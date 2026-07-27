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

    public async Task<IEnumerable<CatalogSidekickDto>> GetSidekicksByHeroAsync(Guid heroId)
    {
        var response = await httpClient.GetAsync($"/sidekick/hero/{heroId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogSidekickDto>>();
    }

    public async Task<Guid> UpdatePlayStyleAsync(CatalogPlayStyleDto playStyle)
    {
        var content = new StringContent(JsonSerializer.Serialize(playStyle), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/hero/{playStyle.HeroId}/playstyle", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    public async Task<CatalogPlayStyleDto> GetPlayStyleByHero(Guid heroId)
    {
        var response = await httpClient.GetAsync($"/hero/{heroId}/playstyle");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CatalogPlayStyleDto>();
    }

    public async Task<IEnumerable<CatalogExpansionDto>> GetExpansionsAsync()
    {
        var response = await httpClient.GetAsync("/expansion");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogExpansionDto>>();
    }

    public async Task<CatalogExpansionDto> UpdateExpansionImageAsync(Guid expansionId, string imageFileName)
    {
        var content = new StringContent(JsonSerializer.Serialize(imageFileName), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/expansion/{expansionId}/image", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CatalogExpansionDto>();
    }

    public async Task<IEnumerable<Guid>> GetOwnedExpansionIdsAsync()
    {
        var response = await httpClient.GetAsync("/collection");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<Guid>>();
    }

    public async Task SetOwnedExpansionIdsAsync(IEnumerable<Guid> expansionIds)
    {
        var content = new StringContent(JsonSerializer.Serialize(expansionIds), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync("/collection", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<CatalogVillainDto>> GetVillainsAsync()
    {
        var response = await httpClient.GetAsync("/villain");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogVillainDto>>();
    }

    public async Task<IEnumerable<CatalogMinionDto>> GetMinionsAsync()
    {
        var response = await httpClient.GetAsync("/minion");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogMinionDto>>();
    }

    public async Task<CatalogHeroDto> UpdateHeroImageAsync(Guid heroId, string imageFileName)
    {
        var content = new StringContent(JsonSerializer.Serialize(imageFileName), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/hero/{heroId}/image", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CatalogHeroDto>();
    }

    public async Task<CatalogVillainDto> UpdateVillainImageAsync(Guid villainId, string imageFileName)
    {
        var content = new StringContent(JsonSerializer.Serialize(imageFileName), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/villain/{villainId}/image", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CatalogVillainDto>();
    }

    public async Task<CatalogMinionDto> UpdateMinionImageAsync(Guid minionId, string imageFileName)
    {
        var content = new StringContent(JsonSerializer.Serialize(imageFileName), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/minion/{minionId}/image", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CatalogMinionDto>();
    }
}