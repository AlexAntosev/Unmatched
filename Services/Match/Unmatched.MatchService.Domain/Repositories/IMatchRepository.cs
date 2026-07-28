namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface IMatchRepository : IRepository<MatchEntity>
{
    MatchEntity Update(MatchEntity model);
    
    Task<List<MatchEntity>> GetFinishedAsync();

    /// <summary>
    /// Finished matches loaded with only the graph a rating replay writes back (see
    /// <see cref="Services.IRatingService.RecalculateAsync"/>), deliberately without the Tournament
    /// navigation: replaying updates every loaded entity, and a Tournament shared by several matches
    /// would both collide in the change tracker and get pointlessly rewritten.
    /// </summary>
    Task<List<MatchEntity>> GetFinishedForRatingReplayAsync();


    Task<List<MatchEntity>> GetFinishedByHeroIdAsync(Guid heroId);
    
    Task<List<MatchEntity>> GetFinishedByMapIdAsync(Guid mapId);
    
    Task<List<MatchEntity>> GetFinishedByPlayerIdAsync(Guid playerId);

    Task<List<MatchEntity>> GetFinishedByVillainIdAsync(Guid villainId);

    Task<List<MatchEntity>> GetFinishedByMinionIdAsync(Guid minionId);

    Task<List<MatchEntity>> GetByTournamentAsync(Guid id);
}
