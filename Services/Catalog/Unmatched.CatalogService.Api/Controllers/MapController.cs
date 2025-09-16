namespace Unmatched.CatalogService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class MapController(IMapper mapper, IMapService mapService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<MapDto>> Get()
    {
        var heroes = await mapService.GetAllAsync();
        var result = heroes.Select(mapper.Map<MapDto>);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<MapDto> Get(Guid id)
    {
        var hero = await mapService.GetAsync(id);
        var result = mapper.Map<MapDto>(hero);
        return result;
    }
}
