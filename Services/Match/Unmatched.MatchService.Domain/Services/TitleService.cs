namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class TitleService(IUnitOfWork unitOfWork, IMapper mapper, ICatalogHeroCache catalogHeroCache) : ITitleService
{
    public async Task AddAsync(Title titleDto)
    {
        var title = mapper.Map<TitleEntity>(titleDto);
        await unitOfWork.Titles.AddAsync(title);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AssignAsync(Guid titleId, Guid heroId)
    {
        var title = await unitOfWork.Titles.GetByIdAsync(titleId);
        var hero = await catalogHeroCache.GetAsync(heroId);

        if (title != null
         && title.HeroTitles.All(h => h.HeroesId != hero.Id))
        {
            title.HeroTitles.Add(
                new HeroTitleEntity
                    {
                        HeroesId = heroId,
                        TitlesId = titleId
                    });
            await unitOfWork.Titles.AddOrUpdateAsync(title);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Titles.Delete(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Title>> GetAsync()
    {
        var entities = await unitOfWork.Titles.GetAsync();
        var titles = mapper.Map<IEnumerable<Title>>(entities);

        return titles;
    }

    public async Task<IEnumerable<HeroTitleAssign>> GetHeroesForTitleAssign(Guid titleId)
    {
        var entities = await catalogHeroCache.GetAsync();
        var title = await unitOfWork.Titles.GetByIdAsync(titleId);

        var heroes = mapper.Map<IEnumerable<HeroTitleAssign>>(entities);

        foreach (var hero in heroes)
        {
            if (title is not null)
            {
                hero.IsAssigned = title.HeroTitles.Any(h => h.HeroesId == hero.Id);
            }
        }

        return heroes;
    }

    public async Task MergeAsync(Guid titleId, IEnumerable<Guid> heroesIds)
    {
        var title = await unitOfWork.Titles.GetByIdAsync(titleId);

        foreach (var titleHero in title.HeroTitles)
        {
            if (heroesIds.All(id => id != titleHero.HeroesId))
            {
                title.HeroTitles.Remove(titleHero);
            }
        }

        foreach (var heroId in heroesIds)
        {
            if (title.HeroTitles.Any(h => h.HeroesId == heroId))
            {
                continue;
            }

            if (title.HeroTitles.All(h => h.HeroesId != heroId))
            {
                title.HeroTitles.Add(
                    new HeroTitleEntity
                        {
                            HeroesId = heroId,
                            TitlesId = titleId
                        });
            }
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task UnassignAsync(Guid titleId, Guid heroId)
    {
        var title = await unitOfWork.Titles.GetByIdAsync(titleId);

        var heroTitleToRemove = title?.HeroTitles.FirstOrDefault(h => h.HeroesId == heroId);
        if (heroTitleToRemove != null)
        {
            title.HeroTitles.Remove(heroTitleToRemove);
            await unitOfWork.Titles.AddOrUpdateAsync(title);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
