namespace Unmatched.CatalogService.Domain.Services;

using System;

using Unmatched.CatalogService.Domain.Entities;

public interface IHeroService
{
    Task<IEnumerable<Hero>> GetAllAsync();

    Task<Hero?> GetAsync(Guid id);
}