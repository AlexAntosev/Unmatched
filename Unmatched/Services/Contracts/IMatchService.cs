namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IMatchService
{
    Task<SaveMatchResultDto> AddAsync(MatchDto match);

    Task<SaveMatchResultDto> UpdateAsync(MatchDto matchDto);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id);

    Task<MatchDto> GetAsync(Guid id);
    
    Task UpdateEpicAsync(Guid matchId, int epic);
}
