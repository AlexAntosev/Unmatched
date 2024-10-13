namespace Unmatched.Services;

using Unmatched.Dtos;

public interface IPlayStyleService
{
    Task AddOrUpdateAsync(Guid heroId, PlayStyleDto playStyleDto);

    Task DeleteAsync(Guid id);

    Task<PlayStyleDto> GetByHeroIdAsync(Guid heroId);
}
