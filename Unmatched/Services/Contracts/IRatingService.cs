namespace Unmatched.Services.Contracts;

public interface IRatingService
{
    public Task RecalculateAsync();
}