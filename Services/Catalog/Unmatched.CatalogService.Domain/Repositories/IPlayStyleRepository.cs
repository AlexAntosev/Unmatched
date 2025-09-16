namespace Unmatched.CatalogService.Domain.Repositories;

using System;

using Unmatched.CatalogService.Domain.Entities;

public interface IPlayStyleRepository : IRepository<PlayStyle>
{
    Task<PlayStyle?> GetByHeroIdAsync(Guid heroId);
}

