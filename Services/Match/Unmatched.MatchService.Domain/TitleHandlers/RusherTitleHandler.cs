namespace Unmatched.MatchService.Domain.TitleHandlers;

using AutoMapper;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Repositories;

public class RusherTitleHandler(IUnitOfWork unitOfWork, IMapper mapper, ICatalogHeroCache catalogHeroCache) : IRusherTitleHandler
{
    private const double MinCardsForTitleRatio = 0.66;

    public async Task<List<Title>> HandleAsync(MatchEntity match)
    {
        var titlesEarned = new List<Title>();
        var titleEntity = await unitOfWork.Titles.GetByNameAsync(Titles.Rusher);
        if (titleEntity is null)
        {
            return titlesEarned;
        }

        var winners = match.Fighters.Where(f => f.IsWinner);

        foreach (var winner in winners)
        {
            var winnerHero = await catalogHeroCache.GetAsync(winner.HeroId);
            var isAlreadyRusher = titleEntity.HeroTitles.Any(h => h.HeroesId == winner.HeroId);
            if (!isAlreadyRusher
             && winner.CardsLeft >= MinCardsForTitleRatio * winnerHero.DeckSize)
            {
                var heroTitle = new HeroTitleEntity
                    {
                        HeroesId = winner.HeroId,
                        TitlesId = titleEntity.Id
                    };
                await unitOfWork.HeroTitles.AddOrUpdateAsync(heroTitle);
                await unitOfWork.SaveChangesAsync();

                titlesEarned.Add(mapper.Map<Title>(titleEntity));
            }
        }

        return titlesEarned;
    }
}
