namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class UnrankedMatchHandler : BaseMatchHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUnrankedRatingCalculator _ratingCalculator;

    public UnrankedMatchHandler(IUnitOfWork unitOfWork, IUnrankedRatingCalculator ratingCalculator)
    {
        _unitOfWork = unitOfWork;
        _ratingCalculator = ratingCalculator;
    }

    protected override async Task InnerHandleAsync(Match match)
    {
        var createdMatch = await _unitOfWork.Matches.AddAsync(match);
        await _unitOfWork.SaveChangesAsync();

        var matchPoints = await _ratingCalculator.CalculateAsync(createdMatch.Fighters.First(), createdMatch.Fighters.Last());

        foreach (var fighter in createdMatch.Fighters)
        {
            UpdateFighterMatchPoints(fighter, matchPoints);
        }

        foreach (var heroMatchPoints in matchPoints)
        {
            await UpdateHeroRatingAsync(heroMatchPoints);
        }

        await _unitOfWork.SaveChangesAsync();
    }
    
    private void UpdateFighterMatchPoints(Fighter fighter, IEnumerable<HeroMatchPoints> matchPoints)
    {
        fighter.MatchPoints = matchPoints.FirstOrDefault(h => h.HeroId == fighter.HeroId).Points;
        _unitOfWork.Fighters.AddOrUpdate(fighter);
    }

    private async Task UpdateHeroRatingAsync(HeroMatchPoints heroMatchPoints)
    {
        var heroRating = await _unitOfWork.Ratings.GetByHeroIdAsync(heroMatchPoints.HeroId)
         ?? new Rating
                {
                    HeroId = heroMatchPoints.HeroId
                };
        
        var matchPoints = heroMatchPoints.Points;
        heroRating.Points += matchPoints;
        
        _unitOfWork.Ratings.AddOrUpdate(heroRating);
    }
}
