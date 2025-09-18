namespace Unmatched.StatisticsService.Domain.Communication.Match.Kafka;

using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Repositories;

public class MatchCreatedHeroHandler(IHeroPlaceAdjuster heroPlaceAdjuster) : IMatchCreatedHandler
{
    public async Task HandleAsync(IUnitOfWork unitOfWork, MatchCreated matchCreated)
    {
        var allHeroStats = await unitOfWork.HeroStats.GetAllAsync();
        foreach (var fighter in matchCreated.Fighters)
        {
            var heroStats = allHeroStats.FirstOrDefault(x => x.HeroId == fighter.HeroId);
            if (heroStats != null)
            {
                heroStats.LastMatchIncludedAt = matchCreated.Date;
                heroStats.ModifiedAt = DateTime.UtcNow;
                heroStats.LastMatchPoints = fighter.MatchPoints ?? 0;
                heroStats.Points = fighter.ResultRating ?? 0;
                heroStats.TotalMatches++;
                heroStats.Place = 999; // TODO: implement
                if (fighter.IsWinner)
                {
                    heroStats.TotalWins++;
                }
                else
                {
                    heroStats.TotalLooses++;
                }
            }
            else
            {
                // TODO: create new entry
                throw new KeyNotFoundException($"There is no hero stats entry found for hero: {fighter.HeroId}");
            }
        }

        var updatedStats = heroPlaceAdjuster.Adjust(allHeroStats);
        await unitOfWork.HeroStats.AddOrUpdateAsync(updatedStats);
    }
}