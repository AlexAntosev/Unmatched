namespace Unmatched.MatchService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.MatchService.Api.Dto;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class TournamentController(ILogger<TournamentController> logger, IMapper mapper, ITournamentService tournamentService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> Get(Guid id)
    {
        var tournament = await tournamentService.GetAsync(id);
        if (tournament != null)
        {
            var dto = mapper.Map<TournamentDto>(tournament);
            return Ok(dto);
        }

        return NotFound();
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> Get()
    {
        var tournaments = await tournamentService.GetAsync();

        var dtos = mapper.Map<IEnumerable<TournamentDto>>(tournaments);
        return Ok(dtos);
    }

    [HttpGet("{id}/standings")]
    public async Task<ActionResult<IEnumerable<TournamentStandingDto>>> GetStandings(Guid id)
    {
        var standings = await tournamentService.GetStandingsAsync(id);
        return Ok(mapper.Map<IEnumerable<TournamentStandingDto>>(standings));
    }

    [HttpPost("generate/{tournamentId}")]
    public async Task<ActionResult> GenerateNextStage(Guid tournamentId)
    {
        await tournamentService.CreateNextStagePlannedMatchesAsync(tournamentId);
        return Ok();
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<TournamentDto>> Complete(Guid id)
    {
        var tournament = await tournamentService.CompleteAsync(id);
        return Ok(mapper.Map<TournamentDto>(tournament));
    }

    [HttpGet("{id}/awards")]
    public async Task<ActionResult<IEnumerable<TournamentAwardDto>>> GetAwards(Guid id)
    {
        var awards = await tournamentService.GetAwardsAsync(id);
        return Ok(mapper.Map<IEnumerable<TournamentAwardDto>>(awards));
    }

    [HttpPost("create")]
    public async Task<ActionResult<TournamentDto>> Create([FromBody] TournamentDto tournament)
    {
        var model = mapper.Map<Tournament>(tournament);
        var addedResult = await tournamentService.AddAsync(model);
        return Ok(mapper.Map<TournamentDto>(addedResult));
    }

    [HttpPut("{id}/image")]
    public async Task<ActionResult<TournamentDto>> UpdateImage(Guid id, [FromBody] string imageFileName)
    {
        var tournament = await tournamentService.UpdateImageAsync(id, imageFileName);
        return tournament is null ? NotFound() : Ok(mapper.Map<TournamentDto>(tournament));
    }

    [HttpPut("{id}/trophy-image")]
    public async Task<ActionResult<TournamentDto>> UpdateTrophyImage(Guid id, [FromBody] string imageFileName)
    {
        var tournament = await tournamentService.UpdateTrophyImageAsync(id, imageFileName);
        return tournament is null ? NotFound() : Ok(mapper.Map<TournamentDto>(tournament));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await tournamentService.DeleteAsync(id);
        return Ok();
    }
}
