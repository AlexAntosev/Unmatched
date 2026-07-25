namespace Unmatched.CatalogService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class ExpansionController(IMapper mapper, IExpansionService expansionService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ExpansionDto>> Get()
    {
        var expansions = await expansionService.GetAllAsync();
        var result = expansions.Select(mapper.Map<ExpansionDto>);
        return result;
    }
}
