namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Models;

public interface IMatchService
{
    Task<SaveMatchResult> AddOrUpdateAsync(Match match);

    Task<IEnumerable<Match>> GetAllAsync();

    Task<IEnumerable<Fighter>> GetAllFightersAsync();

    Task<Match> GetAsync(Guid id);

    Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid id);

    Task<IEnumerable<Fighter>> GetFightersByHeroAsync(Guid heroId);

    Task<IEnumerable<MatchLog>> GetFinishedByHeroAsync(Guid id);

    Task<IEnumerable<MatchLog>> GetFinishedByMapAsync(Guid mapId);

    Task<IEnumerable<MatchLog>> GetFinishedByPlayerAsync(Guid playerId);

    Task<IEnumerable<MatchLog>> GetMatchLogAsync();

    Task UpdateEpicAsync(Guid matchId, int epic);
}
