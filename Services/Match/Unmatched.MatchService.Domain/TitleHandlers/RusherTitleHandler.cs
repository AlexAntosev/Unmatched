namespace Unmatched.MatchService.Domain.TitleHandlers;

using AutoMapper;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class RusherTitleHandler : IRusherTitleHandler
{
    private const double MinCardsForTitleRatio = 0.66;
    
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public RusherTitleHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<TitleDto?> HandleAsync(MatchEntity match)
    {
        var title = await _unitOfWork.Titles.GetByNameAsync(Titles.Rusher);
        if (title is null)
        {
            return null;
        }

        var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);

        // if (winner is not null)
        // {
        //     var winnerHero = await _unitOfWork.Heroes.GetByIdAsync(winner.HeroId);
        //     var isAlreadyRusher = title.Heroes.Any(h => h.Id == winner.HeroId);
        //     if (!isAlreadyRusher && winner.CardsLeft >= MinCardsForTitleRatio * winnerHero.DeckSize)
        //     {
        //         title.Heroes.Add(winnerHero);
        //         _unitOfWork.Titles.AddOrUpdate(title);
        //         await _unitOfWork.SaveChangesAsync();
        //
        //         var titleDto = _mapper.Map<TitleDto>(title);
        //         return titleDto;
        //     }
        // }

        return null;
    }
}
