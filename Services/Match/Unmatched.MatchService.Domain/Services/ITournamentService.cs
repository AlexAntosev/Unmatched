namespace Unmatched.MatchService.Domain.Services;

using System;

using Unmatched.MatchService.Domain.Dto;

public interface ITournamentService
{
    Task<Tournament> AddAsync(Tournament dto);

    Task<IEnumerable<Tournament>> GetAsync();
    
    Task<Tournament> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task CreateInitialPlannedMatchesAsync(Tournament tournament);

    Task CreateNextStagePlannedMatchesAsync(Tournament tournament);

    Task CreateThirdPlaceDeciderMatchAsync(Tournament tournament);

    Task CreateGrandFinalMatchesAsync(Tournament tournament);
}
