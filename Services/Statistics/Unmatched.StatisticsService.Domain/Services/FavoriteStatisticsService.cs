// namespace Unmatched.StatisticsService.Domain.Services;
//
// using AutoMapper;
//
// using Unmatched.StatisticsService.Domain.Catalog;
// using Unmatched.StatisticsService.Domain.Match;
// using Unmatched.StatisticsService.Domain.Models;
// using Unmatched.StatisticsService.Domain.Services.Contracts;
//
// public class FavoriteStatisticsService : IFavoriteStatisticsService
// {
//     private readonly IUnitOfWork _unitOfWork;
//
//     private readonly IMapper _mapper;
//
//     private readonly ICatalogHeroCache _catalogHeroCache;
//
//     public FavoriteStatisticsService(IUnitOfWork unitOfWork, IMapper mapper, ICatalogHeroCache catalogHeroCache)
//     {
//         _unitOfWork = unitOfWork;
//         _mapper = mapper;
//         _catalogHeroCache = catalogHeroCache;
//     }
//     
//     public async Task<IEnumerable<FavoriteHeroStats>> GetFavoritesStatisticsAsync(Guid playerId)
//     {
//         var favorites = await _unitOfWork.Favorites.GetByPlayerIdAsync(playerId);
//         var heroes = await _catalogHeroCache.GetAsync();
//         var player = await _unitOfWork.Players.GetByIdAsync(playerId);
//         var playerDto = _mapper.Map<PlayerDto>(player);
//
//         var statistics = new List<FavoriteHeroStats>();
//         foreach (var hero in heroes)
//         {
//             var existingFavoriteHero = favorites.FirstOrDefault(x => x.HeroId == hero.Id);
//             
//             var heroDto = _mapper.Map<HeroDto>(hero);
//             var fights = await _unitOfWork.Fighters.GetFromFinishedMatchesByHeroAndPlayerIdAsync(hero.Id, playerId);
//             var fightsDto = _mapper.Map<List<FighterDto>>(fights);
//             
//             var favoriteStatistics = new FavoriteHeroStats 
//                 {
//                     FavoriteId = existingFavoriteHero?.Id ?? Guid.NewGuid(),
//                     Hero = heroDto,
//                     HeroId = hero.Id,
//                     Player = playerDto,
//                     PlayerId = playerId,
//                     Fights = fightsDto,
//                     IsChosenOne = existingFavoriteHero?.IsChosenOne ?? default,
//                     Favour = existingFavoriteHero?.Favour ?? default,
//                     TotalMatches = fights.Count,
//                     TotalWins = fights.Count(x => x.IsWinner),
//                     TotalLooses = fights.Count(x => x.IsWinner == false)
//                 };
//             
//             if (fights.Count > 0)
//             {
//                 statistics.Add(favoriteStatistics);
//             }
//         }
//
//         return statistics;
//     }
// }
