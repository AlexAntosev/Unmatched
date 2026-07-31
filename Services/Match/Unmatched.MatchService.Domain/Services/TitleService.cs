namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Models;
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
            if (title.Exclusivity == TitleExclusivity.Unique)
            {
                // a Unique title has exactly one holder - assigning a new one transfers it away from
                // whoever held it before, rather than creating a second holder.
                foreach (var previousHolder in title.HeroTitles.ToList())
                {
                    title.HeroTitles.Remove(previousHolder);
                }
            }

            title.HeroTitles.Add(
                new HeroTitleEntity
                    {
                        HeroesId = heroId,
                        TitlesId = titleId,
                        EarnedAt = DateTime.UtcNow
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

    public async Task<IEnumerable<Title>> GetByHeroAsync(Guid heroId)
    {
        var entities = await unitOfWork.Titles.GetByHeroId(heroId);

        return mapper.Map<IEnumerable<Title>>(entities);
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
        var requestedHeroIds = heroesIds.ToList();

        if (title.Exclusivity == TitleExclusivity.Unique && requestedHeroIds.Count > 1)
        {
            throw new InvalidOperationException("A Unique title can only have one holder - assign it to a single hero to transfer it.");
        }

        // materialise the removals first: HeroTitles can't be mutated while it's being enumerated.
        foreach (var stale in title.HeroTitles.Where(titleHero => requestedHeroIds.All(id => id != titleHero.HeroesId)).ToList())
        {
            title.HeroTitles.Remove(stale);
        }

        foreach (var heroId in requestedHeroIds)
        {
            if (title.HeroTitles.Any(h => h.HeroesId == heroId))
            {
                continue;
            }

            title.HeroTitles.Add(
                new HeroTitleEntity
                    {
                        HeroesId = heroId,
                        TitlesId = titleId,
                        EarnedAt = DateTime.UtcNow
                    });
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
