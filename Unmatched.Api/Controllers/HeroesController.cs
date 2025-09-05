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
public class HeroesController : ControllerBase
{
    private readonly IHeroService _heroService;
    private readonly IHeroStatisticsService _heroStatisticsService;

    public HeroesController(IHeroService heroService, IHeroStatisticsService heroStatisticsService)
    {
        _heroService = heroService;
        _heroStatisticsService = heroStatisticsService;
    }

    [HttpGet]
    public Task<IEnumerable<HeroDto>> Get()
    {
        return _heroService.GetAsync();
    }

    [HttpGet("statistics")]
    public Task<IEnumerable<HeroStatisticsDto>> GetStatistics()
    {
        return _heroStatisticsService.GetHeroesStatisticsAsync();
    }

    [HttpGet("statistics/{heroId}")]
    public Task<HeroStatisticsDto> GetStatistics(Guid heroId)
    {
        return _heroStatisticsService.GetHeroStatisticsAsync(heroId);
    }
}
