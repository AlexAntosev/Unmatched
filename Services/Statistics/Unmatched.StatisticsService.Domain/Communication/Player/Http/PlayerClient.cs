namespace Unmatched.StatisticsService.Domain.Communication.Player.Http;

using System.Net.Http.Json;
using Unmatched.StatisticsService.Domain.Communication.Player.Http.Dto;

public class PlayerClient(HttpClient httpClient) : IPlayerClient
{
    public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
    {
        var response = await httpClient.GetAsync("/player");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PlayerDto>>();
    }
}
