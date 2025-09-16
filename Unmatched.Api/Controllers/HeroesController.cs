namespace Unmatched.Api.Controllers;

using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;

using Unmatched.Dtos;
using Unmatched.Services.Contracts;
using Unmatched.Services.Statistics;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class HeroesController(IHeroService heroService, IHeroStatisticsService heroStatisticsService, IPlayStyleService playStyleService) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<UiHeroDto>> Get()
    {
        return heroService.GetAsync();
    }

    [HttpGet("statistics")]
    public Task<IEnumerable<UiHeroStatisticsDto>> GetStatistics()
    {
        return heroStatisticsService.GetHeroesStatisticsAsync();
    }

    [HttpGet("statistics/{heroId}")]
    public Task<UiHeroStatisticsDto> GetStatistics(Guid heroId)
    {
        return heroStatisticsService.GetHeroStatisticsAsync(heroId);
    }

    [HttpPut("playstyle/{heroId}")]
    public async Task UpdatePlaystyle(Guid heroId, UiPlayStyleDto playStyle)
    {
        await playStyleService.AddOrUpdateAsync(heroId, playStyle);
    }
}
