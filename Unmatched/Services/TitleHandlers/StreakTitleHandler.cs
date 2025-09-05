namespace Unmatched.Services.TitleHandlers;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;

public class StreakTitleHandler : IStreakTitleHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public StreakTitleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task HandleAsync()
    {
        var title = await _unitOfWork.Titles.GetByNameAsync(Titles.Streak);
        if (title is null)
        {
            return;
        }

        // var heroesStreaks = new Dictionary<Hero, int>();
        // var heroes = await _unitOfWork.Heroes.GetAsync();
        // var fighters = await _unitOfWork.Fighters.GetAsync();
        //
        // foreach (var hero in heroes)
        // {
        //     var heroFights = fighters.Where(f => f.HeroId == hero.Id);
        //     var heroStreak = 0;
        //     var winsCounter = 0;
        //     foreach (var heroFight in heroFights)
        //     {
        //         if (heroFight.IsWinner)
        //         {
        //             winsCounter++;
        //
        //             if (winsCounter > heroStreak)
        //             {
        //                 heroStreak = winsCounter;
        //             }
        //         }
        //
        //         if (!heroFight.IsWinner)
        //         {
        //             winsCounter = 0;
        //         }
        //     }
        //     
        //     heroesStreaks.Add(hero, heroStreak);
        // }
        //
        // var heroWithBestStreak = heroesStreaks.OrderByDescending(h => h.Value).FirstOrDefault();
        //
        // title.Heroes = new List<Hero>
        //     {
        //         heroWithBestStreak.Key
        //     };
        // title.Comment = $"({heroWithBestStreak.Value} wins in a row)";
        // _unitOfWork.Titles.AddOrUpdate(title);
        await _unitOfWork.SaveChangesAsync();
    }
}
