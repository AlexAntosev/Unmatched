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

    public async Task<Title?> HandleAsync(MatchEntity match)
    {
        var titleEntity = await unitOfWork.Titles.GetByNameAsync(Titles.Rusher);
        if (titleEntity is null)
        {
            return null;
        }

        var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);

        if (winner is not null)
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

                var title = mapper.Map<Title>(titleEntity);
                return title;
            }
        }

        return null;
    }
}
