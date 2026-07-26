namespace Unmatched.StatisticsService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.StatisticsService.Api.Dto;
using Unmatched.StatisticsService.Domain.Services.Contracts;

[ApiController]
[Route("[controller]")]
public class VillainController(IVillainStatisticsService villainService, IMapper mapper) : ControllerBase
{
    [HttpGet("{villainId}")]
    public async Task<ActionResult<VillainStatsDto>> Get(Guid villainId)
    {
        var stats = await villainService.GetVillainStatisticsAsync(villainId);
        var result = mapper.Map<VillainStatsDto>(stats);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VillainStatsDto>>> Get()
    {
        var stats = await villainService.GetVillainsStatisticsAsync();
        var result = stats.Select(mapper.Map<VillainStatsDto>);
        return Ok(result);
    }
}
