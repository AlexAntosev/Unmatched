namespace Unmatched.CatalogService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class VillainController(IMapper mapper, IVillainService villainService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<VillainDto>> Get()
    {
        var villains = await villainService.GetAllAsync();
        var result = villains.Select(mapper.Map<VillainDto>);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<VillainDto> Get(Guid id)
    {
        var villain = await villainService.GetAsync(id);
        var result = mapper.Map<VillainDto>(villain);
        return result;
    }

    [HttpPut("{id}/image")]
    public async Task<ActionResult<VillainDto>> UpdateImage(Guid id, [FromBody] string imageFileName)
    {
        var villain = await villainService.UpdateImageAsync(id, imageFileName);
        return villain is null ? NotFound() : Ok(mapper.Map<VillainDto>(villain));
    }
}
