namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Models;
using Unmatched.Repositories;

public abstract class BaseMatchHandler : IMatchHandler
{
    protected readonly IUnitOfWork UnitOfWork;

    protected BaseMatchHandler(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    public Task HandleAsync(Match match)
    {
        Validate(match);
        return InnerHandleAsync(match);
    }

    protected abstract Task InnerHandleAsync(Match match);
    
    protected abstract void InnerValidate(Match match);

    protected async Task<Match> CreateMatch(Match match, HeroMatchPoints[] matchPoints)
    {
        foreach (var fighter in match.Fighters)
        {
            fighter.MatchPoints = GetFighterMatchPoints(matchPoints, fighter.HeroId);
        }

        var createdMatch = await UnitOfWork.Matches.AddAsync(match);

        var updatedHeroRatings = new List<Rating>();
        foreach (var heroMatchPoints in matchPoints)
        {
            var rating = await UpdateHeroRatingAsync(heroMatchPoints);
            updatedHeroRatings.Add(rating);
        }

        foreach (var updatedHeroRating in updatedHeroRatings)
        {
            UnitOfWork.Ratings.AddOrUpdate(updatedHeroRating);
        }

        return createdMatch;
    }
    
    private void Validate(Match match)
    {
        if (IsNotEnoughFighters(match.Fighters))
        {
            throw new ArgumentException("Not enough fighters.", nameof(match));
        }

        InnerValidate(match);
    }
    
    private static bool IsNotEnoughFighters(ICollection<Fighter>? fighters) 
        => fighters is null || fighters.Count < 2;
    
    private static int GetFighterMatchPoints(IEnumerable<HeroMatchPoints> matchPoints, Guid heroId) 
        => matchPoints.FirstOrDefault(h => h.HeroId == heroId).Points;

    private async Task<Rating> UpdateHeroRatingAsync(HeroMatchPoints heroMatchPoints)
    {
        var rating = await UnitOfWork.Ratings.GetByHeroIdAsync(heroMatchPoints.HeroId)
         ?? new Rating
                {
                    HeroId = heroMatchPoints.HeroId
                };
        
        rating.Points += heroMatchPoints.Points;
        
        return rating;
    }

    public void Dispose() 
        => UnitOfWork.Dispose();
}

