namespace Unmatched.Services.TitleHandlers;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;

public class RusherTitleHandler : IRusherTitleHandler
{
    private const double MinCardsForTitleRatio = 0.66;
    
    private readonly IUnitOfWork _unitOfWork;

    public RusherTitleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task HandleAsync(Match match)
    {
        var title = await _unitOfWork.Titles.GetByNameAsync(Titles.Rusher);
        if (title is null)
        {
            return;
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
            }
        }
    }
}
