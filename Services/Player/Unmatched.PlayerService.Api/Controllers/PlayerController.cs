namespace Unmatched.PlayerService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.PlayerService.Api.Dto;
using Unmatched.PlayerService.Domain.Entities;
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

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Add([FromBody] PlayerDto dto)
    {
        var player = mapper.Map<Player>(dto);
        await playerService.AddAsync(player);
        return Ok(mapper.Map<PlayerDto>(player));
    }

    [HttpGet("{playerId}/favorites")]
    public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetFavoritesAsync(Guid playerId)
    {
        var favorites = await favoriteService.GetFavoritesAsync(playerId);
        var dtos = favorites.Select(mapper.Map<FavoriteDto>);
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

    [HttpPut("{playerId}/image")]
    public async Task<ActionResult<PlayerDto>> UpdateImage(Guid playerId, [FromBody] string imageFileName)
    {
        var player = await playerService.UpdateImageAsync(playerId, imageFileName);
        return player is null ? NotFound() : Ok(mapper.Map<PlayerDto>(player));
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
