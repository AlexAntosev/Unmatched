namespace Unmatched.Data.Repositories;

public interface IRepository<T>
{
    Task<T> AddAsync(T model);

    void AddOrUpdate(T model);

    Task AddRangeAsync(IEnumerable<T> models);

    Task Delete(Guid id);
    
    void DeleteAll();

    Task<T> GetByIdAsync(Guid id);

    IQueryable<T> Query();

    Task<List<T>> GetAsync();

    Task SaveChangesAsync();
}
