namespace Unmatched.MatchService.Domain.TitleHandlers;

using AutoMapper;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class PunisherTitleHandler : IPunisherTitleHandler
{
    private const double MinVictoryPointsForTitle = 1000;
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PunisherTitleHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<TitleDto?> HandleAsync(MatchEntity match)
    {
        var title = await _unitOfWork.Titles.GetByNameAsync(Titles.Punisher);
        if (title is null)
        {
            return null;
        }

        var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);

        if (winner is not null)
        {
            var isAlreadyPunisher = title.HeroTitles.Any(h => h.HeroesId == winner.HeroId);
            if (!isAlreadyPunisher && winner.MatchPoints >= MinVictoryPointsForTitle)
            {
                // add to HeroTitle rep
                // title.Heroes.Add(winner.Hero);
                // _unitOfWork.Titles.AddOrUpdate(title);
                // await _unitOfWork.SaveChangesAsync();

                var titleDto = _mapper.Map<TitleDto>(title);
                return titleDto;
            }
        }

        return null;
    }
}
