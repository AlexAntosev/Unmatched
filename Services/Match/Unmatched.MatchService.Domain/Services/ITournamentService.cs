namespace Unmatched.MatchService.Domain.Services;

using System;

using Unmatched.MatchService.Domain.Dto;

public interface ITournamentService
{
    Task<TournamentDto> AddAsync(TournamentDto dto);

    Task<IEnumerable<TournamentDto>> GetAsync();
    
    Task<TournamentDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task CreateInitialPlannedMatchesAsync(TournamentDto tournament);

    Task CreateNextStagePlannedMatchesAsync(TournamentDto tournament);

    Task CreateThirdPlaceDeciderMatchAsync(TournamentDto tournament);

    Task CreateGrandFinalMatchesAsync(TournamentDto tournament);
}
