namespace Unmatched.CatalogService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.Domain.Services;

[ApiController]
[Route("[controller]")]
public class SidekickController(IMapper mapper, ISidekickService sidekickService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<SidekickDto>> GetSidekicks()
    {
        var sidekicks = await sidekickService.GetAsync();
        var result = mapper.Map<IEnumerable<SidekickDto>>(sidekicks);
        return result;
    }
}
