namespace Unmatched.MatchService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.MatchService.Api.Dto;
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

    [HttpPost("generate/{tournamentId}")]
    public async Task<ActionResult> GenerateNextStage(Guid tournamentId)
    {
        await tournamentService.CreateNextStagePlannedMatchesAsync(tournamentId);
        return Ok();
    }

    [HttpPost("create")]
    public async Task<ActionResult<TournamentDto>> GenerateNextStage([FromBody] TournamentDto tournament)
    {
        var model = mapper.Map<Domain.Dto.Tournament>(tournament);
        var addedResult = await tournamentService.AddAsync(model);
        return Ok(mapper.Map<TournamentDto>(addedResult));
    }
}
