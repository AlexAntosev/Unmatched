namespace Unmatched.CatalogService.Domain.Services;

using System;

using Unmatched.CatalogService.Domain.Entities;

public interface IVillainService
{
    Task<IEnumerable<Villain>> GetAllAsync();

    Task<Villain?> GetAsync(Guid id);

    Task<Villain?> UpdateImageAsync(Guid id, string imageFileName);
}
