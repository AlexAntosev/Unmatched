namespace Unmatched.MatchService.Domain.Communication.Player;

using Unmatched.MatchService.Domain.Communication.Player.Dto;

public interface IPlayerClient
{
    Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
}
