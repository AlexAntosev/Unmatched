namespace Unmatched.CatalogService.Api.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Unmatched.CatalogService.Api.Dto;
using Unmatched.CatalogService.EntityFramework.Repositories;

[ApiController]
[Route("[controller]")]
public class HeroController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<HeroDto>> Get()
    {
        var heroes = await unitOfWork.Heroes.Query().ToListAsync();
        var result = heroes.Select(mapper.Map<HeroDto>);
        return result;
    }
}
