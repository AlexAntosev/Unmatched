namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Dtos;

public class HeroService : IHeroService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HeroService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<HeroDto>> GetAsync()
    {
        var entities = await _unitOfWork.Heroes.GetAsync();
        var heroes = _mapper.Map<IEnumerable<HeroDto>>(entities);

        return heroes;
    }

    public async Task<IEnumerable<HeroTitleAssignDto>> GetHeroesForTitleAssign(Guid titleId)
    {
        var entities = await _unitOfWork.Heroes.GetAsync();
        var title = await _unitOfWork.Titles.GetByIdAsync(titleId);

        var heroes = _mapper.Map<IEnumerable<HeroTitleAssignDto>>(entities);

        foreach (var hero in heroes)
        {
            if (title is not null)
            {
                hero.IsAssigned = title.Heroes.Any(h => h.Id == hero.Id);
            }
        }

        return heroes;
    }

    public async Task<HeroDto> GetFavouriteHeroAsync(Guid playerId)
    {
        var favourites = (await _unitOfWork.Favorites.GetByPlayerIdAsync(playerId)).OrderByDescending(x => x.IsChosenOne).ThenByDescending(x => x.Favour).ToArray();
        
        var hero = favourites.Any()
            ? favourites.First().Hero
            : _unitOfWork.Heroes.Query(true).FirstOrDefault();
        
        var result = _mapper.Map<HeroDto>(hero);

        if (hero != null)
        {
            var sidekicks = _unitOfWork.Sidekicks.GetByHero(hero.Id);
            result.Sidekicks = _mapper.Map<IEnumerable<SidekickDto>>(sidekicks);
        }

        return result;
    }
}
