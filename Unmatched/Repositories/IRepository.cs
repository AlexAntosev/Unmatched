namespace Unmatched.Repositories;

public interface IRepository<T>
{
    Task<T> AddAsync(T model);

    void AddOrUpdate(T model);

    Task AddRangeAsync(IEnumerable<T> models);

    Task Delete(Guid id);

    Task<T> GetByIdAsync(Guid id);

    IQueryable<T> Query();

    Task SaveChangesAsync();
}
