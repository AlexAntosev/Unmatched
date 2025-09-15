namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;

public interface IMatchService
{
    Task<SaveMatchResult> AddOrUpdateAsync(Match match);

    Task<IEnumerable<MatchLog>> GetMatchLogAsync();

    Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid id);

    Task<IEnumerable<MatchLog>> GetFinishedByHeroAsync(Guid id);

    Task<Match> GetAsync(Guid id);
    
    Task UpdateEpicAsync(Guid matchId, int epic);

    Task<IEnumerable<MatchLog>> GetFinishedByMapAsync(Guid mapId);

    Task<IEnumerable<MatchLog>> GetFinishedByPlayerAsync(Guid playerId);
}
