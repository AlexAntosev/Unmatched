namespace Unmatched.CatalogService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Unmatched.CatalogService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class CollectionController(IOwnedExpansionService ownedExpansionService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Guid>> Get()
    {
        return await ownedExpansionService.GetOwnedExpansionIdsAsync();
    }

    [HttpPut]
    public async Task Put([FromBody] IEnumerable<Guid> expansionIds)
    {
        await ownedExpansionService.ReplaceAsync(expansionIds);
    }
}
