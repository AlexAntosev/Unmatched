using Unmatched.HttpClients.Contracts;

namespace Unmatched.HttpClients;

using Unmatched.Dtos.Match;

public class MatchClient : IMatchClient
{
    public Task<SaveMatchResultDto> AddAsync(MatchDto match)
    {
        throw new NotImplementedException();
    }

    public Task<SaveMatchResultDto> UpdateAsync(MatchDto match)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MatchLogDto>> GetMatchLogAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateEpicAsync(Guid matchId, int epic)
    {
        throw new NotImplementedException();
    }

    public Task<MatchDto> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<MatchDto> GetByTournamentIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
