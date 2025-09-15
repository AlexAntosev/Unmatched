namespace Unmatched.MatchService.Domain.TitleHandlers;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class StreakTitleHandler(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache) : IStreakTitleHandler
{
    public async Task HandleAsync()
    {
        var title = await unitOfWork.Titles.GetByNameAsync(Titles.Streak);
        if (title is null)
        {
            return;
        }

        var heroesStreaks = new Dictionary<Guid, int>();
        var heroes = await catalogHeroCache.GetAsync();
        var fighters = await unitOfWork.Fighters.GetAsync();

        foreach (var hero in heroes)
        {
            var heroFights = fighters.Where(f => f.HeroId == hero.Id);
            var heroStreak = 0;
            var winsCounter = 0;
            foreach (var heroFight in heroFights)
            {
                if (heroFight.IsWinner)
                {
                    winsCounter++;

                    if (winsCounter > heroStreak)
                    {
                        heroStreak = winsCounter;
                    }
                }

                if (!heroFight.IsWinner)
                {
                    winsCounter = 0;
                }
            }

            heroesStreaks.Add(hero.Id, heroStreak);
        }

        var heroWithBestStreak = heroesStreaks.OrderByDescending(h => h.Value).FirstOrDefault();

        title.HeroTitles = new List<HeroTitleEntity>
            {
                new()
                    {
                        HeroesId = heroWithBestStreak.Key,
                        TitlesId = title.Id
                    }
            };
        title.Comment = $"({heroWithBestStreak.Value} wins in a row)";
        await unitOfWork.Titles.AddOrUpdateAsync(title);
        await unitOfWork.SaveChangesAsync();
    }
}
