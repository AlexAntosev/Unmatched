using Microsoft.AspNetCore.Mvc;
using Unmatched.Dtos;
using Unmatched.Services;
using System.Net.Mime;

namespace Unmatched.Api.Controllers;

using Unmatched.Dtos.Match;
using Unmatched.Services.Contracts;
using Unmatched.Services.Statistics;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]    
[Produces(MediaTypeNames.Application.Json)]
public class MatchesController(IMatchService matchService, IHeroStatisticsService heroStatisticsService) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<UiMatchLogDto>> Get()
    {
        return matchService.GetMatchLogAsync();
    }
    
    [HttpGet("{heroId}")]
    public Task<IEnumerable<UiMatchLogDto>> Get(Guid heroId)
    {
        return heroStatisticsService.GetHeroMatchesAsync(heroId);
    }
}
