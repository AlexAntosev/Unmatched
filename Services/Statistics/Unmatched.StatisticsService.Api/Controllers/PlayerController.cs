namespace Unmatched.StatisticsService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.StatisticsService.Api.Dto;
using Unmatched.StatisticsService.Domain.Services.Contracts;

[ApiController]
[Route("[controller]")]
public class PlayerController(ILogger<PlayerController> logger, IPlayerStatisticsService playerStatisticsService, IMapper mapper) : ControllerBase
{
    [HttpGet("{playerId}")]
    public async Task<ActionResult<PlayerStatsDto>> Get(Guid playerId)
    {
        var stats = await playerStatisticsService.GetPlayerStatisticsAsync(playerId);
        var result = mapper.Map<PlayerStatsDto>(stats);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerStatsDto>>> Get()
    {
        var stats = await playerStatisticsService.GetPlayersStatisticsAsync();
        var result = stats.Select(mapper.Map<PlayerStatsDto>);
        return Ok(result);
    }
}
