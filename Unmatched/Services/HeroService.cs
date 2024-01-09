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
        var entities = await _unitOfWork.Heroes.Query().Include(e => e.Sidekicks).OrderBy(x => x.Name).ToListAsync();
        var heroes = _mapper.Map<IEnumerable<HeroDto>>(entities);

        return heroes;
    }

    public async Task<IEnumerable<HeroTitleAssignDto>> GetHeroesForTitleAssign(Guid titleId)
    {
        var entities = await _unitOfWork.Heroes.Query().OrderBy(h => h.Name).ToListAsync();
        var title = await _unitOfWork.Titles.Query().Include(t => t.Heroes).FirstOrDefaultAsync(t => t.Id == titleId);

        var heroes = _mapper.Map<IEnumerable<HeroTitleAssignDto>>(entities);

        foreach (var hero in heroes)
        {
            if (title is not null)
            {
                hero.IsAssigned = title.Heroes.Any(h => h.Id == hero.Id);
            }
        }

        return heroes;
    }
}
