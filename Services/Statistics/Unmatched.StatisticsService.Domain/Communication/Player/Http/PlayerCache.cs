namespace Unmatched.StatisticsService.Domain.Communication.Player.Http;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Player.Http.Dto;

public class PlayerCache(IPlayerClient playerClient) : InMemoryCachedService<PlayerDto>, IPlayerCache
{
    protected override Guid GetId(PlayerDto entity)
    {
        return entity.Id;
    }

    protected override Task<IEnumerable<PlayerDto>> LoadCacheAsync()
    {
        return playerClient.GetAllPlayersAsync();
    }
}
