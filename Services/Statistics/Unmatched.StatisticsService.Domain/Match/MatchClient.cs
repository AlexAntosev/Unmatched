namespace Unmatched.StatisticsService.Domain.Match;

using System.Net.Http.Json;

public class MatchClient(HttpClient httpClient) : IMatchClient
{
    public async Task<IEnumerable<FighterDto>> GetAllFightersAsync()
    {
        var response = await httpClient.GetAsync("/match/fighters");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<FighterDto>>();
    }

    public async Task<IEnumerable<RatingDto>> GetAllRatingsAsync()
    {
        var response = await httpClient.GetAsync("/rating");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<RatingDto>>();
    }

    public async Task<IEnumerable<FighterDto>> GetFightersByHeroAsync(Guid heroId)
    {
        var response = await httpClient.GetAsync($"/match/fighters/hero/{heroId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<FighterDto>>();
    }

    public async Task<RatingDto> GetHeroRatingAsync(Guid heroId)
    {
        var response = await httpClient.GetAsync($"/rating/hero/{heroId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RatingDto>();
    }
}
