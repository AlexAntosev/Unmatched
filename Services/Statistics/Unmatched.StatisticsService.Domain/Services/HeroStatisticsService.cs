namespace Unmatched.StatisticsService.Domain.Services;

using AutoMapper;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Services.Contracts;

public class HeroStatisticsService(IMapper mapper, ICatalogHeroCache catalogHeroCache, IMatchClient matchClient) : IHeroStatisticsService
{
    public async Task<IEnumerable<HeroStats>> GetHeroesStatisticsAsync()
    {
        var heroes = await catalogHeroCache.GetAsync();
        var ratings = await matchClient.GetAllRatingsAsync();
        var fighters = await matchClient.GetAllFightersAsync();

        var statistics = new List<HeroStats>();

        foreach (var hero in heroes)
        {
            var points = ratings.FirstOrDefault(x => x.HeroId.Equals(hero.Id))?.Points ?? 0;
            var fights = fighters.Where(x => x.HeroId.Equals(hero.Id)).OrderByDescending(x => x.DateTime).ToArray();

            var heroStatistics = mapper.Map<HeroStats>(hero);
            heroStatistics.Points = points;
            heroStatistics.TotalMatches = fights.Count();
            heroStatistics.TotalWins = fights.Count(x => x.IsWinner);
            heroStatistics.TotalLooses = fights.Count(x => x.IsWinner == false);
            heroStatistics.LastMatchPoints = fights.OrderByDescending(x => x.DateTime).FirstOrDefault()?.MatchPoints ?? 0;

            statistics.Add(heroStatistics);
        }

        return statistics;
    }

    public async Task<HeroStats> GetHeroStatisticsAsync(Guid heroId)
    {
        var hero = await catalogHeroCache.GetAsync(heroId);
        var fights = await matchClient.GetFightersByHeroAsync(heroId);
        var rating = await matchClient.GetHeroRatingAsync(hero.Id);
        var points = rating?.Points ?? 0;

        var statistics = mapper.Map<HeroStats>(hero);
        statistics.Points = points;
        statistics.Place = 999; // store locally
        statistics.TotalMatches = fights.Count();
        statistics.TotalWins = fights.Count(x => x.IsWinner);
        statistics.TotalLooses = fights.Count(x => x.IsWinner == false);
        statistics.LastMatchPoints = fights.OrderByDescending(x => x.DateTime).FirstOrDefault()?.MatchPoints ?? 0;

        return statistics;
    }
}