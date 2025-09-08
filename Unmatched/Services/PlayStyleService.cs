namespace Unmatched.Services;

using Unmatched.Dtos;
using Unmatched.HttpClients;
using Unmatched.Services.Contracts;

public class PlayStyleService(ICatalogClient catalogClient) : IPlayStyleService
{
    public async Task AddOrUpdateAsync(Guid heroId, PlayStyleDto playStyleDto)
    {
        await catalogClient.UpdatePlayStyleAsync(playStyleDto);
    }
}
