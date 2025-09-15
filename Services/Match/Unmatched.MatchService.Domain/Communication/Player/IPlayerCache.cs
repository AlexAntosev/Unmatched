namespace Unmatched.MatchService.Domain.Communication.Player;

using Unmatched.MatchService.Domain.Cache;
using Unmatched.MatchService.Domain.Communication.Player.Dto;

public interface IPlayerCache : ICachedService<PlayerDto>
{

}