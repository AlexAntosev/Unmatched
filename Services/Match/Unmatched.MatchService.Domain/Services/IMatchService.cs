namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;

public interface IMatchService
{
    Task<SaveMatchResultDto> AddAsync(MatchEntity match);
    
    Task<SaveMatchResultDto> AddAsync(MatchDto match);

    Task<SaveMatchResultDto> UpdateAsync(MatchDto matchDto);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id);

    Task<MatchDto> GetAsync(Guid id);
    
    Task UpdateEpicAsync(Guid matchId, int epic);
}
