namespace Unmatched.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Repositories;

public class HeroService : IHeroService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HeroService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<HeroDto>> GetAsync()
    {
        var entities = await _unitOfWork.Heroes.Query().ToListAsync();
        var heroes = _mapper.Map<IEnumerable<HeroDto>>(entities);

        return heroes;
    }
}
