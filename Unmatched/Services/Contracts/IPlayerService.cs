namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;
using Unmatched.Dtos.Player;

public interface IPlayerService
{
    Task<IEnumerable<UiPlayerDto>> GetAsync();

    Task AddAsync(UiPlayerDto dto);

    Task<string> UpdateImageAsync(Guid playerId, string imageFileName);
}