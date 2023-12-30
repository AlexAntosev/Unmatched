namespace Unmatched.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Repositories;

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
        var entities = await _unitOfWork.Titles.Query().Include(t => t.Heroes).ToListAsync();
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
        var title = await _unitOfWork.Titles.GetByIdAsync(titleId);
        var hero = await _unitOfWork.Heroes.GetByIdAsync(heroId);

        if (title.Heroes.All(h => h.Id != hero.Id))
        {
            title.Heroes.Add(hero);
            _unitOfWork.Titles.AddOrUpdate(title);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task UnassignAsync(Guid titleId, Guid heroId)
    {
        var title = await _unitOfWork.Titles.GetByIdAsync(titleId);
        var hero = await _unitOfWork.Heroes.GetByIdAsync(heroId);
        
        if (title.Heroes.Any(h => h.Id == hero.Id))
        {
            title.Heroes.Remove(hero);
            _unitOfWork.Titles.AddOrUpdate(title);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task MergeAsync(Guid titleId, IEnumerable<Guid> heroesIds)
    {
        var title = await _unitOfWork.Titles.GetByIdAsync(titleId);

        foreach (var titleHero in title.Heroes)
        {
            if (heroesIds.All(id => id != titleHero.Id))
            {
                title.Heroes.Remove(titleHero);
            }
        }

        foreach (var heroId in heroesIds)
        {
            if (title.Heroes.Any(h => h.Id == heroId))
            {
                continue;
            }

            if (title.Heroes.All(h => h.Id != heroId))
            {
                var hero = await _unitOfWork.Heroes.GetByIdAsync(heroId);
                title.Heroes.Add(hero);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
