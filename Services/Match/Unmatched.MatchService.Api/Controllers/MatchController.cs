namespace Unmatched.MatchService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.MatchService.Api.Dto;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class MatchController(ILogger<MatchController> logger, IMapper mapper, IMatchService matchService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SaveMatchResultDto>> AddMatch([FromBody] MatchDto match)
    {
        var saveResult = await matchService.AddOrUpdateAsync(mapper.Map<Match>(match));
        if (saveResult != null)
        {
            return Ok(mapper.Map<SaveMatchResultDto>(saveResult));
        }
        else
        {
            return BadRequest(); // refactor
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MatchDto>> Get(Guid id)
    {
        var match = await matchService.GetAsync(id);
        if (match != null)
        {
            var dto = mapper.Map<MatchDto>(match);
            return Ok(dto);
        }

        return NotFound();
    }

    [HttpGet("log")]
    public async Task<ActionResult<IEnumerable<MatchLogDto>>> Get()
    {
        var matches = await matchService.GetMatchLogAsync();

        var dtos = mapper.Map<IEnumerable<MatchLogDto>>(matches);
        return Ok(dtos);
    }

    [HttpGet("tournament/{tournamentId}")]
    public async Task<ActionResult<IEnumerable<MatchLogDto>>> GetByTournament(Guid tournamentId)
    {
        var matches = await matchService.GetByTournamentIdAsync(tournamentId);

        var dtos = mapper.Map<IEnumerable<MatchDto>>(matches);
        return Ok(dtos);
    }

    [HttpPut("{id}/epic")]
    public Task UpdateEpic(Guid id, [FromBody] UpdateEpicDto epic)
    {
        return matchService.UpdateEpicAsync(id, epic.EpicValue);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SaveMatchResultDto>> UpdateMatch(Guid id, [FromBody] MatchDto match)
    {
        var saveResult = await matchService.AddOrUpdateAsync(mapper.Map<Match>(match));
        if (saveResult != null)
        {
            return Ok(mapper.Map<SaveMatchResultDto>(saveResult));
        }
        else
        {
            return BadRequest(); // refactor
        }
    }
}

[ApiController]
[Route("[controller]")]
public class RatingController(IRatingService ratingService) : ControllerBase
{
    [HttpPost("recalculate")]
    public async Task<ActionResult> Recalculate()
    {
        await ratingService.RecalculateAsync();
        return Ok();
    }
}
