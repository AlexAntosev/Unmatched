namespace Unmatched.DataInitialization;

public interface IDataInitializer<T>
{
    IEnumerable<T> GetEntities();

    Task InitializeAsync();
}