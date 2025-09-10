namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;

public interface IMatchService
{
    Task<SaveMatchResult> AddOrUpdateAsync(Match match);

    Task<IEnumerable<MatchLog>> GetMatchLogAsync();

    Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid id);

    Task<Match> GetAsync(Guid id);
    
    Task UpdateEpicAsync(Guid matchId, int epic);
}
