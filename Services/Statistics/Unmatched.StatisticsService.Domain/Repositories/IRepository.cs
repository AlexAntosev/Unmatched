namespace Unmatched.StatisticsService.Domain.Repositories;

public interface IRepository<T>
{
    Task<T> AddAsync(T model);

    Task AddOrUpdateAsync(T model);

    Task AddRangeAsync(IEnumerable<T> models);

    Task DeleteAsync(Guid id);

    void DeleteAll();

    IReadOnlyList<T> GetAll();

    Task<IReadOnlyList<T>> GetAllAsync();

    Task<T?> GetAsync(Guid id);

    Task SaveChangesAsync();
}
