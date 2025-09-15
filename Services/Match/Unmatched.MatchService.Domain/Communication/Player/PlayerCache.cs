namespace Unmatched.MatchService.Domain.Communication.Player;

using Unmatched.MatchService.Domain.Cache;
using Unmatched.MatchService.Domain.Communication.Player.Dto;

public class PlayerCache(IPlayerClient playerClient) : InMemoryCachedService<PlayerDto>, IPlayerCache
{
    protected override Guid GetId(PlayerDto entity)
    {
        return entity.Id;
    }

    protected override async Task<IEnumerable<PlayerDto>> LoadCacheAsync()
    {
        return await playerClient.GetAllPlayersAsync();
    }
}
