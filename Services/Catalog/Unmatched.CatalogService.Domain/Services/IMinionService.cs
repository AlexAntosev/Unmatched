namespace Unmatched.CatalogService.Domain.Services;

using System;

using Unmatched.CatalogService.Domain.Entities;

public interface IMinionService
{
    Task<IEnumerable<Minion>> GetAllAsync();

    Task<Minion?> GetAsync(Guid id);
}
