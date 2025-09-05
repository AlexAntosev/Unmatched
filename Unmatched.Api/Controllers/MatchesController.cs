using Microsoft.AspNetCore.Mvc;
using Unmatched.Dtos;
using Unmatched.Services;
using System.Net.Mime;

namespace Unmatched.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]    
[Produces(MediaTypeNames.Application.Json)]
public class MatchesController(IMatchService matchService) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<MatchLogDto>> Get()
    {
        return matchService.GetMatchLogAsync();
    }
}
