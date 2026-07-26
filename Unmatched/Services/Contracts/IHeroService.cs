namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IHeroService
{
    Task<IEnumerable<UiHeroDto>> GetAsync();

    Task<string> UpdateImageAsync(Guid heroId, string imageFileName);
}
