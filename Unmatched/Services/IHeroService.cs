namespace Unmatched.Services;

using Unmatched.Dtos;

public interface IHeroService
{
    Task<IEnumerable<HeroDto>> GetAsync();

    Task<IEnumerable<HeroTitleAssignDto>> GetHeroesForTitleAssign(Guid titleId);
    Task<HeroDto> GetFavouriteHeroAsync(Guid playerId);
}
