namespace Unmatched.MatchService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.MatchService.Api.Dto;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class TitleController(ITitleService titleService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TitleDto>>> Get()
    {
        var titles = await titleService.GetAsync();
        return Ok(titles.Select(mapper.Map<TitleDto>));
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] TitleDto title)
    {
        await titleService.AddAsync(mapper.Map<Title>(title));
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await titleService.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("{titleId}/merge")]
    public async Task<ActionResult> Merge(Guid titleId, [FromBody] IEnumerable<Guid> heroesIds)
    {
        await titleService.MergeAsync(titleId, heroesIds);
        return Ok();
    }

    [HttpGet("{titleId}/heroes")]
    public async Task<ActionResult<IEnumerable<HeroTitleAssignDto>>> GetHeroesForTitleAssign(Guid titleId)
    {
        var heroes = await titleService.GetHeroesForTitleAssign(titleId);
        return Ok(heroes.Select(mapper.Map<HeroTitleAssignDto>));
    }
}
