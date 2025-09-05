namespace Unmatched.Services;

using Unmatched.Dtos;
using Unmatched.Dtos.Catalog;

public interface ICatalogHeroCache : IInMemoryCachedService
{
    Task<IEnumerable<CatalogHeroDto>> GetAsync();
    Task<CatalogHeroDto> GetAsync(Guid id);
}

public interface ICachedService
{

}

public interface IInMemoryCachedService : ICachedService
{

}
