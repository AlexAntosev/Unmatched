namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos.Match;

public interface IMatchClient
{
    Task<SaveMatchResultDto> AddAsync(MatchDto match);

    Task<SaveMatchResultDto> UpdateAsync(MatchDto match);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task UpdateEpicAsync(Guid matchId, int epic);

    Task<MatchDto> GetAsync(Guid id);

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id);
}