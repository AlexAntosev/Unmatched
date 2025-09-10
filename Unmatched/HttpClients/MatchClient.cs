using Unmatched.HttpClients.Contracts;

namespace Unmatched.HttpClients;

using System.Net.Http;
using System.Net.Http.Json;

using Unmatched.Dtos.Match;

public class MatchClient(HttpClient httpClient) : IMatchClient
{
    public Task<SaveMatchResultDto> AddAsync(MatchDto match)
    {
        throw new NotImplementedException();
    }

    public Task<SaveMatchResultDto> UpdateAsync(MatchDto match)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        var response = await httpClient.GetAsync("/match/log");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<MatchLogDto>>();
    }

    public Task UpdateEpicAsync(Guid matchId, int epic)
    {
        throw new NotImplementedException();
    }

    public Task<MatchDto> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task RecalculateAsync()
    {
        throw new NotImplementedException();
    }
}
