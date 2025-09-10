namespace Unmatched.MatchService.Domain.TitleHandlers;

using AutoMapper;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class PunisherTitleHandler(IUnitOfWork unitOfWork, IMapper mapper) : IPunisherTitleHandler
{
    private const double MinVictoryPointsForTitle = 1000;

    public async Task<Title?> HandleAsync(MatchEntity match)
    {
        var title = await unitOfWork.Titles.GetByNameAsync(Titles.Punisher);
        if (title is null)
        {
            return null;
        }

        var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);

        if (winner is not null)
        {
            var isAlreadyPunisher = title.HeroTitles.Any(h => h.HeroesId == winner.HeroId);
            if (!isAlreadyPunisher
             && winner.MatchPoints >= MinVictoryPointsForTitle)
            {
                title.HeroTitles.Add(
                    new HeroTitleEntity()
                        {
                            HeroesId = winner.HeroId,
                            TitlesId = title.Id
                        });
                await unitOfWork.Titles.AddOrUpdateAsync(title);
                await unitOfWork.SaveChangesAsync();

                var titleDto = mapper.Map<Title>(title);
                return titleDto;
            }
        }

        return null;
    }
}
