namespace Unmatched.MatchService.Domain.Repositories;

public interface IRepository<T>
{
    Task<T> AddAsync(T model);

    void AddOrUpdate(T model, Guid id);

    void AddOrUpdate(T model);

    Task AddOrUpdateAsync(T model);

    Task AddRangeAsync(IEnumerable<T> models);

    Task Delete(Guid id);

    void DeleteAll();

    IReadOnlyList<T> Get();

    Task<IReadOnlyList<T>> GetAsync();

    Task<T?> GetByIdAsync(Guid id);

    Task SaveChangesAsync();
}
