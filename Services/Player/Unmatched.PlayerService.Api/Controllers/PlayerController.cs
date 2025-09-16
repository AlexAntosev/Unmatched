namespace Unmatched.PlayerService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.PlayerService.Api.Dto;
using Unmatched.PlayerService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class PlayerController(
    ILogger<PlayerController> logger,
    IPlayerService playerService,
    IFavoriteService favoriteService,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> Get()
    {
        var players = await playerService.GetAsync();
        var dtos = players.Select(mapper.Map<PlayerDto>);
        return Ok(dtos);
    }

    [HttpGet("{playerId}/favor")]
    public async Task<ActionResult<Guid?>> GetFavouriteHeroIdAsync(Guid playerId)
    {
        var heroId = await favoriteService.GetFavouriteHeroIdAsync(playerId);
        if (heroId != null)
        {
            return Ok(heroId);
        }

        return NotFound();
    }

    [HttpPut("{playerId}/hero/{heroId}/chosen")]
    public async Task<ActionResult<Guid>> UpdateChosenOneAsync(Guid playerId, Guid heroId, [FromBody] bool isChosenOne)
    {
        try
        {
            var chosenHeroResult = await favoriteService.UpdateChosenOneAsync(playerId, heroId, isChosenOne);

            return chosenHeroResult != null
                ? Ok(chosenHeroResult)
                : BadRequest();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not update chosen hero");
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{playerId}/hero/{heroId}/favor")]
    public async Task<ActionResult> UpdateFavourAsync(Guid playerId, Guid heroId, [FromBody] int favour)
    {
        try
        {
            await favoriteService.UpdateFavourAsync(playerId, heroId, favour);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not update favor");
            return BadRequest(e.Message);
        }
    }
}
