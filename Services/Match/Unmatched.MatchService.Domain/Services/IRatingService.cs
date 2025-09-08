namespace Unmatched.MatchService.Domain.Services;

public interface IRatingService
{
    public Task RecalculateAsync();
}
