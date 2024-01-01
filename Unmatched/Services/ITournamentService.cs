namespace Unmatched.Services;

using System;

using Unmatched.Dtos;
using Unmatched.Entities;

public interface ITournamentService
{
    Task AddAsync(TournamentDto dto);
    
    Task UpdateAsync(Guid id, IEnumerable<MatchDto> matches, Stage stage);

    Task<IEnumerable<TournamentDto>> GetAsync();
    
    Task<TournamentDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);
}
