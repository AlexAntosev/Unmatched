namespace Unmatched.Services;

using System;

using Unmatched.Dtos;

public interface ITournamentService
{
    Task AddAsync(TournamentDto dto);

    Task<IEnumerable<TournamentDto>> GetAsync();
    
    Task<TournamentDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);
}
