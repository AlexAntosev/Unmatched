namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class ExpansionService(IMapper mapper, ICatalogClient catalogClient) : IExpansionService
{
    public async Task<IEnumerable<ExpansionDto>> GetAsync()
    {
        var entities = await catalogClient.GetExpansionsAsync();
        var expansions = mapper.Map<IEnumerable<ExpansionDto>>(entities);
        return expansions;
    }
}
