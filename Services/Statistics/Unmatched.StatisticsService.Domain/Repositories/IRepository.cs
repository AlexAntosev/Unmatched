namespace Unmatched.StatisticsService.Domain.Repositories;

public interface IRepository<T>
{
    Task<T> AddAsync(T model);

    Task AddOrUpdateAsync(T model);

    Task AddOrUpdateAsync(IEnumerable<T> models);

    Task AddRangeAsync(IEnumerable<T> models);

    void DeleteAll();

    Task DeleteAllAsync();

    Task DeleteAsync(Guid id);

    IReadOnlyList<T> GetAll();

    Task<IReadOnlyList<T>> GetAllAsync();

    Task<T?> GetAsync(Guid id);

    Task<bool> HasRecordsAsync();

    Task SaveChangesAsync();
}
