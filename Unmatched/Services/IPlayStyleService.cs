namespace Unmatched.Services;

using System;

using Unmatched.Dtos;

public interface IPlayStyleService
{
    Task AddAsync(PlayStyleDto playStyle);

    Task<PlayStyleDto> GetByHeroIdAsync(Guid heroId);

    Task UpdateAsync(Guid heroId, PlayStyleDto playStyleDto);

    Task DeleteAsync(Guid id);
}
