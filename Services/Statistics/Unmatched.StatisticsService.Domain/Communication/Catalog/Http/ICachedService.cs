namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

public interface ICachedService<T>
{
    Task<IEnumerable<T>> GetAsync();

    Task<T?> GetAsync(Guid id);
}
