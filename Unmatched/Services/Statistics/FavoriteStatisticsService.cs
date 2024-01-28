namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Data.Repositories;
using Unmatched.Dtos;

public class FavoriteStatisticsService : IFavoriteStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public FavoriteStatisticsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<FavoriteStatisticsDto>> GetFavoritesStatisticsAsync(Guid playerId)
    {
        var favorites = await _unitOfWork.Favorites.GetByPlayerIdAsync(playerId);
        var player = await _unitOfWork.Players.GetByIdAsync(playerId);
        var playerDto = _mapper.Map<PlayerDto>(player);

        var statistics = new List<FavoriteStatisticsDto>();
        foreach (var favorite in favorites)
        {
            var heroDto = _mapper.Map<HeroDto>(favorite.Hero);
            var fights = await _unitOfWork.Fighters.GetFromFinishedMatchesByHeroAndPlayerIdAsync(favorite.HeroId, playerId);
            var fightsDto = _mapper.Map<List<FighterDto>>(fights);

            var favoriteStatistics = new FavoriteStatisticsDto 
                {
                    FavoriteId = favorite.Id,
                    Hero = heroDto,
                    HeroId = favorite.HeroId,
                    Player = playerDto,
                    PlayerId = playerId,
                    Fights = fightsDto,
                    IsChosenOne = favorite.IsChosenOne,
                    Favour = favorite.Favour,
                    TotalMatches = fights.Count,
                    TotalWins = fights.Count(x => x.IsWinner),
                    TotalLooses = fights.Count(x => x.IsWinner == false)
                };

            if (fights.Count > 0)
            {
                statistics.Add(favoriteStatistics);
            }
        }

        return statistics;
    }
}
