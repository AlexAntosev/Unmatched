namespace Unmatched.StatisticsService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.StatisticsService.Api.Dto;
using Unmatched.StatisticsService.Domain.Services.Contracts;

[ApiController]
[Route("[controller]")]
public class MapController(ILogger<MapController> logger, IMapStatisticsService mapService, IMapper mapper) : ControllerBase
{
    [HttpGet("{mapId}")]
    public async Task<ActionResult<MapStatsDto>> Get(Guid mapId)
    {
        var stats = await mapService.GetMapStatisticsAsync(mapId);
        var result = mapper.Map<MapStatsDto>(stats);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MapStatsDto>>> Get()
    {
        var stats = await mapService.GetMapsStatisticsAsync();
        var result = stats.Select(mapper.Map<MapStatsDto>).ToList();
        return Ok(result);
    }
}
