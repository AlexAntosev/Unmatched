namespace Unmatched.Services.Contracts;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;
using Unmatched.HttpClients.Contracts;

public class MatchService(IMatchClient matchClient, IMapper mapper) : IMatchService
{
    public async Task<SaveMatchResultDto> AddAsync(UiMatchDto match)
    {
        var dto = mapper.Map<MatchDto>(match);
        return await matchClient.AddAsync(dto);
    }

    public async Task<SaveMatchResultDto> UpdateAsync(UiMatchDto match)
    {
        var dto = mapper.Map<MatchDto>(match);
        return await matchClient.UpdateAsync(dto);
    }

    public async Task<IEnumerable<UiMatchLogDto>> GetMatchLogAsync()
    {
        var dtos = await matchClient.GetMatchLogAsync();
        var result = mapper.Map<IEnumerable<UiMatchLogDto>>(dtos);
        return result;
    }

    public async Task<IEnumerable<UiMatchDto>> GetByTournamentIdAsync(Guid id)
    {
        var dtos = await matchClient.GetByTournamentIdAsync(id);
        var result = mapper.Map<IEnumerable<UiMatchDto>>(dtos);
        return result;
    }

    public async Task<UiMatchDto> GetAsync(Guid id)
    {
        var dto = await matchClient.GetAsync(id);
        var result = mapper.Map<UiMatchDto>(dto);
        return result;
    }

    public Task UpdateEpicAsync(Guid matchId, int epic)
    {
        return matchClient.UpdateEpicAsync(matchId, epic);
    }
}
