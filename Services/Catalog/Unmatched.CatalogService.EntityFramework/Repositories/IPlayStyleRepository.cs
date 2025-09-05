namespace Unmatched.CatalogService.EntityFramework.Repositories;

using System;

using Unmatched.CatalogService.EntityFramework.Entities;
using Unmatched.Common.EntityFramework;

public interface IPlayStyleRepository : IRepository<PlayStyle>
{
    Task<PlayStyle?> GetByHeroIdAsync(Guid heroId);
}

