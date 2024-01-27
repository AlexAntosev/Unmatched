namespace Unmatched.Data.Repositories;

using Unmatched.Data.Entities;

public interface IFighterRepository : IRepository<Fighter>
{
    Task<List<Fighter>> GetByMatchIdAsync(Guid matchId);
    
    Task<List<Fighter>> GetFromFinishedMatchesAsync();
    
    Task<List<Fighter>> GetFromFinishedMatchesByHeroIdAsync(Guid heroId);
    
    Task<List<Fighter>> GetFromFinishedMatchesByPlayerIdAsync(Guid playerId);

    Task<List<Fighter>> GetFromFinishedMatchesByHeroAndPlayerIdAsync(Guid heroId, Guid playerId);
}
