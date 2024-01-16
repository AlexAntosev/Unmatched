namespace Unmatched.Services.TitleHandlers;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;

public class PunisherTitleHandler : IPunisherTitleHandler
{
    private const double MinVictoryPointsForTitle = 1000;
    
    private readonly IUnitOfWork _unitOfWork;

    public PunisherTitleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task HandleAsync(Match match)
    {
        var title = await _unitOfWork.Titles.GetByNameAsync(Titles.Punisher);
        if (title is null)
        {
            return;
        }

        var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);

        if (winner is not null)
        {
            var isAlreadyPunisher = title.Heroes.Any(h => h.Id == winner.HeroId);
            if (!isAlreadyPunisher && winner.MatchPoints >= MinVictoryPointsForTitle)
            {
                title.Heroes.Add(winner.Hero);
                _unitOfWork.Titles.AddOrUpdate(title);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
