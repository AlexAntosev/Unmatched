namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class HeroService(IMapper mapper, ICatalogClient catalogClient) : IHeroService
{
    public async Task<IEnumerable<UiHeroDto>> GetAsync()
    {
        var entities = await catalogClient.GetHeroesAsync();
        var heroes = mapper.Map<IEnumerable<UiHeroDto>>(entities);

        return heroes;
    }
}
