﻿namespace Unmatched.Repositories;

using Unmatched.Entities;
using Unmatched.Enums;

public interface IMatchRepository : IRepository<Match>
{
    Match Update(Match model);
    
    Task<List<Match>> GetFinishedAsync();
    
    Task<List<Match>> GetFinishedByHeroIdAsync(Guid heroId);
    
    Task<List<Match>> GetFinishedByMapIdAsync(Guid mapId);
    
    Task<List<Match>> GetFinishedByPlayerIdAsync(Guid playerId);
    
    Task<List<Match>> GetByTournamentAndStageAsync(Guid id, Stage? stage = null);
}
