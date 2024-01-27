namespace Unmatched.Services;

using Unmatched.Data.Entities;
using Unmatched.Data.Enums;
using Unmatched.Dtos;

public interface IMatchService
{
    Task<SaveMatchResultDto> AddAsync(Match match);
    
    Task<SaveMatchResultDto> AddAsync(MatchDto match);

    Task<SaveMatchResultDto> UpdateAsync(MatchDto matchDto);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id);

    Task<MatchDto> GetAsync(Guid id);
    
    Task UpdateEpicAsync(Guid matchId, int epic);
}
