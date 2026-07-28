namespace Unmatched.StatisticsService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.StatisticsService.Api.Dto;
using Unmatched.StatisticsService.Domain.Services.Contracts;

[ApiController]
[Route("[controller]")]
public class ExpansionController(IExpansionStatisticsService expansionStatisticsService, IMapper mapper) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<ActionResult<IEnumerable<ExpansionStatsDto>>> Get()
    {
        var stats = await expansionStatisticsService.GetExpansionsStatisticsAsync();
        return Ok(stats.Select(mapper.Map<ExpansionStatsDto>));
    }
}
