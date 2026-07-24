namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class FavoriteStatisticsService(IMapper mapper, IMatchClient matchClient, IPlayerClient playerClient, ICatalogClient catalogClient) : IFavoriteStatisticsService
{
    public async Task<IEnumerable<FavoriteStatisticsDto>> GetFavoritesStatisticsAsync(Guid playerId)
    {
        var matches = await matchClient.GetFinishedByPlayerAsync(playerId);
        var favorites = (await playerClient.GetFavoritesAsync(playerId)).ToList();
        var heroes = (await catalogClient.GetHeroesAsync()).ToList();

        var playerFights = matches
            .SelectMany(match => match.Fighters)
            .Where(fighter => fighter.Player?.Id == playerId)
            .ToList();

        var result = new List<FavoriteStatisticsDto>();

        foreach (var heroFights in playerFights.GroupBy(fighter => fighter.HeroId))
        {
            var hero = heroes.FirstOrDefault(h => h.Id == heroFights.Key);
            if (hero is null)
            {
                continue;
            }

            var favorite = favorites.FirstOrDefault(f => f.HeroId == heroFights.Key);
            var fights = heroFights.ToList();

            result.Add(
                new FavoriteStatisticsDto
                    {
                        HeroId = heroFights.Key,
                        Hero = mapper.Map<UiHeroDto>(hero),
                        PlayerId = playerId,
                        Fights = fights,
                        IsChosenOne = favorite?.IsChosenOne ?? false,
                        Favour = favorite?.Favour ?? 0,
                        TotalMatches = fights.Count,
                        TotalWins = fights.Count(f => f.IsWinner),
                        TotalLooses = fights.Count(f => f.IsWinner == false)
                    });
        }

        return result;
    }
}
