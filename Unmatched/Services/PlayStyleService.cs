namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Dtos.Catalog;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class PlayStyleService(ICatalogClient catalogClient, IMapper mapper) : IPlayStyleService
{
    public async Task AddOrUpdateAsync(Guid heroId, UiPlayStyleDto playStyleDto)
    {
        await catalogClient.UpdatePlayStyleAsync(mapper.Map<CatalogPlayStyleDto>(playStyleDto));
    }
}
