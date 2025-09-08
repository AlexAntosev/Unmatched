namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class TitleService : ITitleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TitleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task AddAsync(TitleDto titleDto)
    {
        var title = _mapper.Map<Title>(titleDto);
        await _unitOfWork.Titles.AddAsync(title);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<TitleDto>> GetAsync()
    {
        var entities = await _unitOfWork.Titles.GetAsync();
        var titles = _mapper.Map<IEnumerable<TitleDto>>(entities);

        return titles;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _unitOfWork.Titles.Delete(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignAsync(Guid titleId, Guid heroId)
    {
        // var title = await _unitOfWork.Titles.GetByIdAsync(titleId);
        // var hero = await _unitOfWork.Heroes.GetByIdAsync(heroId);
        //
        // if (title.Heroes.All(h => h.Id != hero.Id))
        // {
        //     title.Heroes.Add(hero);
        //     _unitOfWork.Titles.AddOrUpdate(title);
        //     await _unitOfWork.SaveChangesAsync();
        // }
    }

    public async Task UnassignAsync(Guid titleId, Guid heroId)
    {
        // var title = await _unitOfWork.Titles.GetByIdAsync(titleId);
        // var hero = await _unitOfWork.Heroes.GetByIdAsync(heroId);
        //
        // if (title.Heroes.Any(h => h.Id == hero.Id))
        // {
        //     title.Heroes.Remove(hero);
        //     _unitOfWork.Titles.AddOrUpdate(title);
        //     await _unitOfWork.SaveChangesAsync();
        // }
    }

    public async Task MergeAsync(Guid titleId, IEnumerable<Guid> heroesIds)
    {
        // var title = await _unitOfWork.Titles.GetByIdAsync(titleId);
        //
        // foreach (var titleHero in title.Heroes)
        // {
        //     if (heroesIds.All(id => id != titleHero.Id))
        //     {
        //         title.Heroes.Remove(titleHero);
        //     }
        // }
        //
        // foreach (var heroId in heroesIds)
        // {
        //     if (title.Heroes.Any(h => h.Id == heroId))
        //     {
        //         continue;
        //     }
        //
        //     if (title.Heroes.All(h => h.Id != heroId))
        //     {
        //         var hero = await _unitOfWork.Heroes.GetByIdAsync(heroId);
        //         title.Heroes.Add(hero);
        //     }
        // }
        //
        // await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<HeroTitleAssignDto>> GetHeroesForTitleAssign(Guid titleId)
    {
        var entities = await _catalogHeroCache.GetAsync();
        var title = await _unitOfWork.Titles.GetByIdAsync(titleId);

        var heroes = _mapper.Map<IEnumerable<HeroTitleAssignDto>>(entities);

        foreach (var hero in heroes)
        {
            if (title is not null)
            {
                hero.IsAssigned = title.HeroTitles.Any(h => h.HeroesId == hero.Id);
            }
        }

        return heroes;
    }
}
