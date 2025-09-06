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

    [HttpGet("{id}")]
    public async Task<HeroDto> Get(Guid id)
    {
        var hero = await unitOfWork.Heroes.GetByIdAsync(id);
        var result = mapper.Map<HeroDto>(hero);
        return result;
    }
}
