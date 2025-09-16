namespace Unmatched.Services.Contracts;

using System;
using Unmatched.Dtos.Match;

public interface ITournamentService
{
    Task<TournamentDto> AddAsync(TournamentDto dto);

    Task<IEnumerable<TournamentDto>> GetAsync();
    
    Task<TournamentDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task GenerateMatchesAsync(Guid tournamentId);
}