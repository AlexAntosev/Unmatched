namespace Unmatched.MatchService.Domain.TitleHandlers;

using AutoMapper;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Repositories;

public class PunisherTitleHandler(IUnitOfWork unitOfWork, IMapper mapper) : IPunisherTitleHandler
{
    private const double MinVictoryPointsForTitle = 1000;

    public async Task<List<Title>> HandleAsync(MatchEntity match)
    {
        var titlesEarned = new List<Title>();
        var title = await unitOfWork.Titles.GetByNameAsync(Titles.Punisher);
        if (title is null)
        {
            return titlesEarned;
        }

        var winners = match.Fighters.Where(f => f.IsWinner);

        foreach (var winner in winners)
        {
            var isAlreadyPunisher = title.HeroTitles.Any(h => h.HeroesId == winner.HeroId);
            if (!isAlreadyPunisher
             && winner.MatchPoints >= MinVictoryPointsForTitle)
            {
                var heroTitle = new HeroTitleEntity
                    {
                        HeroesId = winner.HeroId,
                        TitlesId = title.Id
                    };
                await unitOfWork.HeroTitles.AddOrUpdateAsync(heroTitle);
                await unitOfWork.SaveChangesAsync();

                titlesEarned.Add(mapper.Map<Title>(title));
            }
        }

        return titlesEarned;
    }
}
