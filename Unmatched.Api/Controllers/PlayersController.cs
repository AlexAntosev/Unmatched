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
public class PlayersController(IPlayerService playerService, IPlayerStatisticsService playerStatisticsService) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<UiPlayerDto>> Get()
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
