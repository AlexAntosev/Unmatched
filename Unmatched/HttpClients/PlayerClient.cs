namespace Unmatched.HttpClients;

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Unmatched.Dtos.Player;
using Unmatched.HttpClients.Contracts;

public class PlayerClient(HttpClient httpClient) : IPlayerClient
{
    public async Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        var response = await httpClient.GetAsync("/player");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PlayerDto>>();
    }

    public async Task<Guid?> GetFavouriteHeroIdAsync(Guid playerId)
    {
        var response = await httpClient.GetAsync($"/player/{playerId}/favor");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid?>();
    }

    public async Task<Guid> UpdateChosenOneAsync(Guid playerId, Guid heroId, bool isChosenOne)
    {
        var content = new StringContent(JsonSerializer.Serialize(isChosenOne), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"/player/{playerId}/hero/{heroId}/chosen", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    public async Task UpdateFavourAsync(Guid playerId, Guid heroId, int favour)
    {
        var content = new StringContent(JsonSerializer.Serialize(favour), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"/player/{playerId}/hero/{heroId}/favor", content);
        response.EnsureSuccessStatusCode();
    }
}
