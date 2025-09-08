namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class MatchService(IMatchClient matchClient) : IMatchService
{
    public Task<SaveMatchResultDto> AddAsync(MatchDto match)
    {
        return matchClient.AddAsync(match);
    }

    public Task<SaveMatchResultDto> UpdateAsync(MatchDto match)
    {
        return matchClient.UpdateAsync(match);
    }

    public Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        return matchClient.GetMatchLogAsync();
    }

    public Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<MatchDto> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateEpicAsync(Guid matchId, int epic)
    {
        throw new NotImplementedException();
    }
}
