namespace Unmatched.HttpClients.Contracts;

public interface IRatingClient
{
    Task RecalculateAsync();
}