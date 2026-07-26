namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class VillainService(IMapper mapper, ICatalogClient catalogClient) : IVillainService
{
    public async Task<IEnumerable<VillainDto>> GetAsync()
    {
        var entities = await catalogClient.GetVillainsAsync();
        return mapper.Map<IEnumerable<VillainDto>>(entities);
    }

    public async Task<string> UpdateImageAsync(Guid villainId, string imageFileName)
    {
        var updated = await catalogClient.UpdateVillainImageAsync(villainId, imageFileName);
        return updated.ImageFileName!;
    }
}
