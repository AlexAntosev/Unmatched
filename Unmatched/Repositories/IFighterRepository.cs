namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface IFighterRepository : IRepository<Fighter>
{
    Task<List<Fighter>> GetByMatchIdAsync(Guid matchId);
    
    Task<List<Fighter>> GetFromFinishedMatchesAsync();
    
    Task<List<Fighter>> GetFromFinishedMatchesByHeroIdAsync(Guid heroId);
    
    Task<List<Fighter>> GetFromFinishedMatchesByPlayerIdAsync(Guid playerId);
}
