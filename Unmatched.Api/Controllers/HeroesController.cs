using Microsoft.AspNetCore.Mvc;
using Unmatched.Dtos;
using Unmatched.Services;
using Unmatched.Services.Statistics;
using System.Net.Mime;

namespace Unmatched.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]    
[Produces(MediaTypeNames.Application.Json)]
public class HeroesController(IHeroService heroService, IHeroStatisticsService heroStatisticsService, IPlayStyleService playStyleService) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<HeroDto>> Get()
    {
        return heroService.GetAsync();
    }

    [HttpGet("statistics")]
    public Task<IEnumerable<HeroStatisticsDto>> GetStatistics()
    {
        return heroStatisticsService.GetHeroesStatisticsAsync();
    }

    [HttpGet("statistics/{heroId}")]
    public Task<HeroStatisticsDto> GetStatistics(Guid heroId)
    {
        return heroStatisticsService.GetHeroStatisticsAsync(heroId);
    }

    [HttpPut("playstyle/{heroId}")]
    public async Task UpdatePlaystyle(Guid heroId, PlayStyleDto playStyle)
    {
        await playStyleService.AddOrUpdateAsync(heroId, playStyle);
    }
}
