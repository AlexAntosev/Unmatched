namespace Unmatched.Services;

using Unmatched.Dtos;

public interface IHeroService
{
    Task<IEnumerable<HeroDto>> GetAsync();
}
