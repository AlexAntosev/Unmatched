using Unmatched.HttpClients.Contracts;

namespace Unmatched.HttpClients;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;
using Unmatched.Enums;

public class MatchClient(HttpClient httpClient) : IMatchClient
{
    public async Task<SaveMatchResultDto> AddAsync(MatchDto match)
    {


        var content = new StringContent(JsonSerializer.Serialize(match), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"/match", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SaveMatchResultDto>();
    }

    public async Task<SaveMatchResultDto> UpdateAsync(MatchDto match)
    {
        var content = new StringContent(JsonSerializer.Serialize(match), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/match/{match.Id}", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SaveMatchResultDto>();
    }

    public async Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        var response = await httpClient.GetAsync("/match/log");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<MatchLogDto>>();
    }

    public async Task UpdateEpicAsync(Guid matchId, int epic)
    {
        var content = new StringContent(JsonSerializer.Serialize(new UpdateEpicDto(epic)), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/match/{matchId}/epic", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<MatchDto> GetAsync(Guid id)
    {
        var response = await httpClient.GetAsync($"/match/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MatchDto>();
    }

    public async Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id)
    {
        var response = await httpClient.GetAsync($"/match/tournament/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<MatchDto>>();
    }

    public async Task RecalculateAsync()
    {
        var response = await httpClient.PutAsync("/rating/recalculate",null);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync()
    {
        var response = await httpClient.GetAsync($"/tournament");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<TournamentDto>>();
    }

    public async Task<TournamentDto> GetTournamentAsync(Guid id)
    {
        var response = await httpClient.GetAsync($"/tournament/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TournamentDto>();
    }
}
