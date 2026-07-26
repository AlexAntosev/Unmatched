namespace Unmatched.StatisticsService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.StatisticsService.Api.Dto;
using Unmatched.StatisticsService.Domain.Services.Contracts;

[ApiController]
[Route("[controller]")]
public class MinionController(IMinionStatisticsService minionService, IMapper mapper) : ControllerBase
{
    [HttpGet("{minionId}")]
    public async Task<ActionResult<MinionStatsDto>> Get(Guid minionId)
    {
        var stats = await minionService.GetMinionStatisticsAsync(minionId);
        var result = mapper.Map<MinionStatsDto>(stats);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MinionStatsDto>>> Get()
    {
        var stats = await minionService.GetMinionsStatisticsAsync();
        var result = stats.Select(mapper.Map<MinionStatsDto>);
        return Ok(result);
    }

    [HttpPut("{id}/image")]
    public async Task<ActionResult<MinionStatsDto>> UpdateImage(Guid id, [FromBody] string imageFileName)
    {
        var stats = await minionService.UpdateImageAsync(id, imageFileName);
        return stats is null ? NotFound() : Ok(mapper.Map<MinionStatsDto>(stats));
    }
}
