namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public abstract class BaseMatchHandler(IUnitOfWork unitOfWork) : IMatchHandler
{
    protected readonly IUnitOfWork UnitOfWork = unitOfWork;

    public Task HandleAsync(MatchEntity match)
    {
        Validate(match);
        return InnerHandleAsync(match);
    }

    protected abstract Task InnerHandleAsync(MatchEntity match);
    
    protected abstract void InnerValidate(MatchEntity match);

    protected async Task<MatchEntity> CreateMatch(MatchEntity match, Dictionary<Guid, int> matchPoints)
    {
        foreach (var fighter in match.Fighters)
        {
            fighter.MatchPoints = matchPoints[fighter.HeroId];
        }

        match.IsPlanned = false;
        var createdMatch = match.Id == Guid.Empty ? await UnitOfWork.Matches.AddAsync(match) : UnitOfWork.Matches.Update(match);

        var updatedHeroRatings = new List<RatingEntity>();
        foreach (var heroMatchPoints in matchPoints)
        {
            var rating = await UpdateHeroRatingAsync(heroMatchPoints.Key, heroMatchPoints.Value);
            updatedHeroRatings.Add(rating);
        }

        foreach (var updatedHeroRating in updatedHeroRatings)
        {
            await UnitOfWork.Ratings.AddOrUpdateAsync(updatedHeroRating);
        }

        return createdMatch;
    }
    
    private void Validate(MatchEntity match)
    {
        if (IsNotEnoughFighters(match.Fighters))
        {
            throw new ArgumentException("Not enough fighters.", nameof(match));
        }
        
        if (IsNotOneWinner(match.Fighters))
        {
            throw new ArgumentException("Match should has one winner.", nameof(match));
        }

        InnerValidate(match);
    }
    
    private static bool IsNotEnoughFighters(ICollection<FighterEntity>? fighters) 
        => fighters is null || fighters.Count < 2;
    
    private static bool IsNotOneWinner(ICollection<FighterEntity> fighters) 
        => fighters.Count(f => f.IsWinner) != 1;

    private async Task<RatingEntity> UpdateHeroRatingAsync(Guid heroId, int matchPoint)
    {
        var rating = await UnitOfWork.Ratings.GetByHeroIdAsync(heroId)
         ?? new RatingEntity
         {
                    HeroId = heroId
                };
        
        rating.Points += matchPoint;
        
        return rating;
    }

    public void Dispose() 
        => UnitOfWork.Dispose();
}

