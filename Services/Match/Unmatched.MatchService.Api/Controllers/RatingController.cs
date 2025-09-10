namespace Unmatched.MatchService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Unmatched.MatchService.Domain.Services;

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
