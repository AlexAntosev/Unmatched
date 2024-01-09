namespace Unmatched.Services;

using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Enums;

public interface IMatchService
{
    Task AddAsync(Match match);
    
    Task AddAsync(MatchDto match, FighterDto fighter, FighterDto opponent);

    Task UpdateAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id, Stage? stage = null);

    Task<MatchDto> GetAsync(Guid id);
}
