namespace Unmatched.Services.TitleHandlers;

using AutoMapper;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Dtos;

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
    
    public async Task<TitleDto?> HandleAsync(Match match)
    {
        var title = await _unitOfWork.Titles.GetByNameAsync(Titles.Rusher);
        if (title is null)
        {
            return null;
        }

        var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);

        if (winner is not null)
        {
            var isAlreadyRusher = title.Heroes.Any(h => h.Id == winner.HeroId);
            if (!isAlreadyRusher && winner.CardsLeft >= MinCardsForTitleRatio * winner.Hero.DeckSize)
            {
                title.Heroes.Add(winner.Hero);
                _unitOfWork.Titles.AddOrUpdate(title);
                await _unitOfWork.SaveChangesAsync();

                var titleDto = _mapper.Map<TitleDto>(title);
                return titleDto;
            }
        }

        return null;
    }
}
