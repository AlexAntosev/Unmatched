namespace Unmatched.CatalogService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class MinionController(IMapper mapper, IMinionService minionService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<MinionDto>> Get()
    {
        var minions = await minionService.GetAllAsync();
        var result = minions.Select(mapper.Map<MinionDto>);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<MinionDto> Get(Guid id)
    {
        var minion = await minionService.GetAsync(id);
        var result = mapper.Map<MinionDto>(minion);
        return result;
    }
}
