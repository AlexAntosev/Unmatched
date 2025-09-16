namespace Unmatched.MatchService.Domain.Services;

using System;

using Unmatched.MatchService.Domain.Models;

public interface ITournamentService
{
    Task<Tournament> AddAsync(Tournament dto);

    Task<IEnumerable<Tournament>> GetAsync();
    
    Task<Tournament> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task CreateNextStagePlannedMatchesAsync(Guid tournamentId);
}
