namespace Unmatched.MatchService.Domain.Communication.Player;

using System.Net.Http.Json;

using Unmatched.MatchService.Domain.Communication.Player.Dto;

public class PlayerClient(HttpClient httpClient) : IPlayerClient
{
    public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
    {
        var response = await httpClient.GetAsync("/player");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<PlayerDto>>();
    }
}
