namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IPlayStyleService
{
    Task AddOrUpdateAsync(Guid heroId, PlayStyleDto playStyleDto);
}
