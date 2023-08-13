namespace Unmatched.DataInitialization;

public interface IDataInitializer<T>
{
    Task InitializeAsync();
    IEnumerable<T> GetEntities();
}