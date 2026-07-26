using Unmatched.HttpClients.Contracts;

namespace Unmatched.HttpClients;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Unmatched.Dtos;
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

    public async Task<PlayerStatisticsDto> GetPlayerStatsAsync(Guid playerId)
    {
        var response = await httpClient.GetAsync($"/player/{playerId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlayerStatisticsDto>();
    }

    public async Task<IEnumerable<PlayerStatisticsDto>> GetPlayerStatsAsync()
    {
        var response = await httpClient.GetAsync("/player");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PlayerStatisticsDto>>();
    }

    public async Task<VillainStatisticsDto> GetVillainStatsAsync(Guid villainId)
    {
        var response = await httpClient.GetAsync($"/villain/{villainId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VillainStatisticsDto>();
    }

    public async Task<IEnumerable<VillainStatisticsDto>> GetVillainStatsAsync()
    {
        var response = await httpClient.GetAsync("/villain");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<VillainStatisticsDto>>();
    }

    public async Task<MinionStatisticsDto> GetMinionStatsAsync(Guid minionId)
    {
        var response = await httpClient.GetAsync($"/minion/{minionId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MinionStatisticsDto>();
    }

    public async Task<IEnumerable<MinionStatisticsDto>> GetMinionStatsAsync()
    {
        var response = await httpClient.GetAsync("/minion");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<MinionStatisticsDto>>();
    }

    public async Task<HeroStatisticsDto> UpdateHeroImageAsync(Guid heroId, string imageFileName)
    {
        var content = new StringContent(JsonSerializer.Serialize(imageFileName), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/hero/{heroId}/image", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<HeroStatisticsDto>();
    }

    public async Task<VillainStatisticsDto> UpdateVillainImageAsync(Guid villainId, string imageFileName)
    {
        var content = new StringContent(JsonSerializer.Serialize(imageFileName), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/villain/{villainId}/image", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VillainStatisticsDto>();
    }

    public async Task<MinionStatisticsDto> UpdateMinionImageAsync(Guid minionId, string imageFileName)
    {
        var content = new StringContent(JsonSerializer.Serialize(imageFileName), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/minion/{minionId}/image", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MinionStatisticsDto>();
    }
}
