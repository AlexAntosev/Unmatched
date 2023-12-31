namespace Unmatched.Services.TitleHandlers;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

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

        var heroesStreaks = new Dictionary<Hero, int>();
        var heroes = await _unitOfWork.Heroes.Query().ToListAsync();
        var fighters = await _unitOfWork.Fighters.Query().Include(f => f.Match).OrderBy(f => f.Match.Date).ToListAsync();
        
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
            
            heroesStreaks.Add(hero, heroStreak);
        }

        var heroWithBestStreak = heroesStreaks.OrderByDescending(h => h.Value).FirstOrDefault();

        title.Heroes = new List<Hero>
            {
                heroWithBestStreak.Key
            };
        _unitOfWork.Titles.AddOrUpdate(title);
        await _unitOfWork.SaveChangesAsync();
    }
}
