namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class GoldenHalatLeagueMatchHandler : BaseMatchHandler
{
    private readonly IRatingCalculator _ratingCalculator;

    public GoldenHalatLeagueMatchHandler(
        IUnitOfWork unitOfWork,
        IRatingCalculator ratingCalculator)
    : base(unitOfWork)
    {
        _ratingCalculator = ratingCalculator;
    }
    
    protected override async Task InnerHandleAsync(Match match)
    {
        var matchPoints = (await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last())).ToArray();

        foreach (var fighter in match.Fighters)
        {
            fighter.MatchPoints = GetFighterMatchPoints(matchPoints, fighter.HeroId);
        }
        
        await UnitOfWork.Matches.AddAsync(match);

        var tasks = matchPoints.Select(UpdateHeroRatingAsync);
        var updatedHeroRatings = await Task.WhenAll(tasks);
        foreach (var updatedHeroRating in updatedHeroRatings)
        {
            UnitOfWork.Ratings.AddOrUpdate(updatedHeroRating);
        }
        
        await UnitOfWork.SaveChangesAsync();
    }

    private static int GetFighterMatchPoints(IEnumerable<HeroMatchPoints> matchPoints, Guid heroId) 
        => matchPoints.FirstOrDefault(h => h.HeroId == heroId).Points;

    private async Task<Rating> UpdateHeroRatingAsync(HeroMatchPoints heroMatchPoints)
    {
        var heroRating = await UnitOfWork.Ratings.GetByHeroIdAsync(heroMatchPoints.HeroId)
         ?? new Rating
                {
                    HeroId = heroMatchPoints.HeroId
                };
        
        heroRating.Points += heroMatchPoints.Points;

        return heroRating;
    }
}
