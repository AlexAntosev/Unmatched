namespace Unmatched.StatisticsService.Domain.Communication.Player.Http;

using Unmatched.StatisticsService.Domain.Communication.Player.Http.Dto;

public interface IPlayerClient
{
    Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
}
