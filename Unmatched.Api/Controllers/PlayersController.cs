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
public class PlayersController(IPlayerService playerService, IPlayerStatisticsService playerStatisticsService) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<PlayerDto>> Get()
    {
        return playerService.GetAsync();
    }

    [HttpGet("statistics")]
    public Task<IEnumerable<PlayerStatisticsDto>> GetStatistics()
    {
        return playerStatisticsService.GetPlayersStatisticsAsync();
    }

    [HttpGet("statistics/{heroId}")]
    public Task<PlayerStatisticsDto> GetStatistics(Guid heroId)
    {
        return playerStatisticsService.GetPlayerStatisticsAsync(heroId);
    }
}
