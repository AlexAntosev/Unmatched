namespace Unmatched.Repositories;

public interface IRepository<T>
{
    Task<T> GetByIdAsync(Guid id);
    
    IEnumerable<T> Query();

    Task<T> AddAsync(T model);
    
    Task AddRangeAsync(IEnumerable<T> models);
    
    Task Delete(Guid id);
    
    Task SaveChangesAsync();
}