namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface IMatchRepository : IRepository<MatchEntity>
{
    MatchEntity Update(MatchEntity model);
    
    Task<List<MatchEntity>> GetFinishedAsync();
    
    Task<List<MatchEntity>> GetFinishedByHeroIdAsync(Guid heroId);
    
    Task<List<MatchEntity>> GetFinishedByMapIdAsync(Guid mapId);
    
    Task<List<MatchEntity>> GetFinishedByPlayerIdAsync(Guid playerId);
    
    Task<List<MatchEntity>> GetByTournamentAsync(Guid id);
}
