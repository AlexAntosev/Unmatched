namespace Unmatched.MatchService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.MatchService.Api.Dto;
using Unmatched.MatchService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class RatingController(IRatingService ratingService, IMapper mapper) : ControllerBase
{
    [HttpGet("hero/{heroId}")]
    public async Task<ActionResult<RatingDto>> GetByHero(Guid heroId)
    {
        var rating = await ratingService.GetByHeroAsync(heroId);
        return Ok(mapper.Map<RatingDto>(rating));
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<RatingDto>>> Get()
    {
        var rating = await ratingService.GetAllAsync();
        return Ok(rating.Select(mapper.Map<RatingDto>));
    }

    [HttpGet("changes/hero/{heroId}")]
    public async Task<ActionResult<IEnumerable<RatingChangeDto>>> GetRatingChanges(Guid heroId)
    {
        var changes = await ratingService.GetRatingChangesAsync(heroId);
        return Ok(changes.Select(mapper.Map<RatingChangeDto>));
    }

    [HttpPost("recalculate")]
    public async Task<ActionResult> Recalculate()
    {
        await ratingService.RecalculateAsync();
        return Ok();
    }
}
