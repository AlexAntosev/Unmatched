namespace Unmatched.CatalogService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class HeroController(IMapper mapper, IHeroService heroService, IPlayStyleService playStyleService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<HeroDto>> Get()
    {
        var heroes = await heroService.GetAllAsync();
        var result = heroes.Select(mapper.Map<HeroDto>);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<HeroDto> Get(Guid id)
    {
        var hero = await heroService.GetAsync(id);
        var result = mapper.Map<HeroDto>(hero);
        return result;
    }

    [HttpGet("{heroId}/playstyle")]
    public async Task<PlayStyleDto> GetPlayStyle(Guid heroId)
    {
        var playStyle = await playStyleService.GetAsync(heroId);
        var result = mapper.Map<PlayStyleDto>(playStyle);
        return result;
    }

    [HttpPut("{heroId}/playstyle")]
    public async Task<ActionResult<Guid>> UpdatePlayStyle(Guid heroId, [FromBody] PlayStyleDto playStyle)
    {
        var addedPlayStyle = await playStyleService.AddOrUpdateAsync(mapper.Map<PlayStyle>(playStyle));
        if (addedPlayStyle != null)
        {
            return Ok(addedPlayStyle.HeroId);
        }
        else
        {
            return BadRequest(); // refactor
        }
    }
}