namespace Unmatched.Services.Contracts;

public interface ICollectionService
{
    Task<IEnumerable<Guid>> GetOwnedExpansionIdsAsync();

    Task SetOwnedExpansionIdsAsync(IEnumerable<Guid> expansionIds);
}
