namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Dto.Catalog;

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
