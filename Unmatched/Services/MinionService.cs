namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class MinionService(IMapper mapper, ICatalogClient catalogClient) : IMinionService
{
    public async Task<IEnumerable<MinionDto>> GetAsync()
    {
        var entities = await catalogClient.GetMinionsAsync();
        return mapper.Map<IEnumerable<MinionDto>>(entities);
    }

    public async Task<string> UpdateImageAsync(Guid minionId, string imageFileName)
    {
        var updated = await catalogClient.UpdateMinionImageAsync(minionId, imageFileName);
        return updated.ImageFileName!;
    }
}
