namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IPlayStyleService
{
    Task AddOrUpdateAsync(Guid heroId, UiPlayStyleDto playStyleDto);

    Task<UiPlayStyleDto?> GetAsync(Guid heroId);
}
