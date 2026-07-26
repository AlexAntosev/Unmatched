namespace Unmatched.MatchService.Domain.Repositories;

public interface IRatingRecalculationStateRepository
{
    Task<bool> IsRecalculationRequiredAsync();

    Task SetRecalculationRequiredAsync(bool isRequired);
}
