namespace Unmatched.CatalogService.Domain.Repositories;

public interface IRepository<T>
{
    Task<T> AddAsync(T model);

    void AddOrUpdate(T model, Guid id);

    void AddOrUpdate(T model);

    Task AddOrUpdateAsync(T model);

    Task AddRangeAsync(IEnumerable<T> models);

    Task Delete(Guid id);

    void DeleteAll();

    Task<IReadOnlyList<T>> GetAsync();

    Task<T?> GetByIdAsync(Guid id);

    IQueryable<T> Query(bool noTrack = false);

    Task SaveChangesAsync();
}
