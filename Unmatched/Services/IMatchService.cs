namespace Unmatched.Services;

using Unmatched.Data.Entities;
using Unmatched.Data.Enums;
using Unmatched.Dtos;

public interface IMatchService
{
    Task AddAsync(Match match);
    
    Task AddAsync(MatchDto match, FighterDto fighter, FighterDto opponent);

    Task UpdateAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id);

    Task<MatchDto> GetAsync(Guid id);
}
