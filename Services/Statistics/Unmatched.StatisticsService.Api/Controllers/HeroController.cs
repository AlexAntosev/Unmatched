namespace Unmatched.StatisticsService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.StatisticsService.Api.Dto;
using Unmatched.StatisticsService.Domain.Services.Contracts;

[ApiController]
[Route("[controller]")]
public class HeroController(ILogger<HeroController> logger, IHeroStatisticsService heroService, IMapper mapper) : ControllerBase
{
    [HttpGet("{heroId}")]
    public async Task<ActionResult<HeroStatsDto>> Get(Guid heroId)
    {
        var stats = await heroService.GetHeroStatisticsAsync(heroId);
        var result = mapper.Map<HeroStatsDto>(stats);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<HeroStatsDto>> Get()
    {
        var stats = await heroService.GetHeroesStatisticsAsync();
        var result = stats.Select(mapper.Map<HeroStatsDto>);
        return Ok(result);
    }
}