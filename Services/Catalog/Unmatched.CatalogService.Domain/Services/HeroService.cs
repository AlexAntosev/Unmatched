namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class HeroService(IUnitOfWork unitOfWork) : IHeroService
{

    public async Task<IEnumerable<Hero>> GetAllAsync()
    {
        return await unitOfWork.Heroes.GetAsync();
    }

    public Task<Hero?> GetAsync(Guid id)
    {
        return unitOfWork.Heroes.GetByIdAsync(id);
    }

    public async Task<Hero?> UpdateImageAsync(Guid id, string imageFileName)
    {
        var hero = await unitOfWork.Heroes.GetByIdAsync(id);
        if (hero is null)
        {
            return null;
        }

        hero.ImageFileName = imageFileName;
        unitOfWork.Heroes.AddOrUpdate(hero, id);
        await unitOfWork.SaveChangesAsync();
        return hero;
    }
}