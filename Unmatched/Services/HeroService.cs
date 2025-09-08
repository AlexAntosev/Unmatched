namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients;
using Unmatched.Services.Contracts;

public class HeroService(IMapper mapper, ICatalogClient catalogClient) : IHeroService
{
    public async Task<IEnumerable<HeroDto>> GetAsync()
    {
        var entities = await catalogClient.GetHeroesAsync();
        var heroes = mapper.Map<IEnumerable<HeroDto>>(entities);

        return heroes;
    }
}
