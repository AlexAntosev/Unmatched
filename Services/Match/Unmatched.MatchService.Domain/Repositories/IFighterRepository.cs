namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface IFighterRepository : IRepository<FighterEntity>
{
    Task<List<FighterEntity>> GetByMatchIdAsync(Guid matchId);
    
    Task<List<FighterEntity>> GetFromFinishedMatchesAsync();
    
    Task<List<FighterEntity>> GetFromFinishedMatchesByHeroIdAsync(Guid heroId);
    
    Task<List<FighterEntity>> GetFromFinishedMatchesByPlayerIdAsync(Guid playerId);

    Task<List<FighterEntity>> GetFromFinishedMatchesByHeroAndPlayerIdAsync(Guid heroId, Guid playerId);
}
