namespace Unmatched.MatchService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.MatchService.Api.Dto;
using Unmatched.MatchService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class RatingController(IRatingService ratingService, IMapper mapper) : ControllerBase
{
    [HttpPost("recalculate")]
    public async Task<ActionResult> Recalculate()
    {
        await ratingService.RecalculateAsync();
        return Ok();
    }

    [HttpGet("changes/hero/{heroId}")]
    public async Task<ActionResult<IEnumerable<RatingChangeDto>>> GetRatingChanges(Guid heroId)
    {
        var changes = await ratingService.GetRatingChangesAsync(heroId);
        return Ok(changes.Select(mapper.Map<RatingChangeDto>));
    }
}
