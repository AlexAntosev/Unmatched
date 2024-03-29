﻿namespace Unmatched.Data.Repositories;

using Unmatched.Data.Entities;

public interface IMatchRepository : IRepository<Match>
{
    Match Update(Match model);
    
    Task<List<Match>> GetFinishedAsync();
    
    Task<List<Match>> GetFinishedByHeroIdAsync(Guid heroId);
    
    Task<List<Match>> GetFinishedByMapIdAsync(Guid mapId);
    
    Task<List<Match>> GetFinishedByPlayerIdAsync(Guid playerId);
    
    Task<List<Match>> GetByTournamentAsync(Guid id);
}
