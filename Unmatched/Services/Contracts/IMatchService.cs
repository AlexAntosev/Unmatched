namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public interface IMatchService
{
    Task<SaveMatchResultDto> AddAsync(UiMatchDto match);

    Task<SaveMatchResultDto> UpdateAsync(UiMatchDto matchDto);

    Task<IEnumerable<UiMatchLogDto>> GetMatchLogAsync();

    Task<IEnumerable<UiMatchDto>> GetByTournamentIdAsync(Guid id);

    Task<UiMatchDto> GetAsync(Guid id);
    
    Task UpdateEpicAsync(Guid matchId, int epic);
}