namespace Unmatched.Services.Contracts;

using Unmatched.Dtos.Player;

public interface IPlayerService
{
    Task<IEnumerable<UiPlayerDto>> GetAsync();
    
    Task AddAsync(UiPlayerDto dto);
}