namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Validation;

public abstract class BaseMatchHandler(IUnitOfWork unitOfWork, IGameModeValidatorFactory validatorFactory) : IMatchHandler
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
            fighter.MatchPoints = matchPoints.TryGetValue(fighter.HeroId, out var points) ? points : 0;
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
        validatorFactory.Create(match.GameMode).Validate(match);

        InnerValidate(match);
    }

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

