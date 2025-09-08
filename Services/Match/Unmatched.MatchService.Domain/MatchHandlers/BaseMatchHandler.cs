namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

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

    protected async Task<Match> CreateMatch(Match match, Dictionary<Guid, int> matchPoints)
    {
        foreach (var fighter in match.Fighters)
        {
            fighter.MatchPoints = matchPoints[fighter.HeroId];
        }

        match.IsPlanned = false;
        var createdMatch = match.Id == Guid.Empty ? await UnitOfWork.Matches.AddAsync(match) : UnitOfWork.Matches.Update(match);

        var updatedHeroRatings = new List<Rating>();
        foreach (var heroMatchPoints in matchPoints)
        {
            var rating = await UpdateHeroRatingAsync(heroMatchPoints.Key, heroMatchPoints.Value);
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
        
        if (IsNotOneWinner(match.Fighters))
        {
            throw new ArgumentException("Match should has one winner.", nameof(match));
        }

        InnerValidate(match);
    }
    
    private static bool IsNotEnoughFighters(ICollection<Fighter>? fighters) 
        => fighters is null || fighters.Count < 2;
    
    private static bool IsNotOneWinner(ICollection<Fighter> fighters) 
        => fighters.Count(f => f.IsWinner) != 1;

    private async Task<Rating> UpdateHeroRatingAsync(Guid heroId, int matchPoint)
    {
        var rating = await UnitOfWork.Ratings.GetByHeroIdAsync(heroId)
         ?? new Rating
                {
                    HeroId = heroId
                };
        
        rating.Points += matchPoint;
        
        return rating;
    }

    public void Dispose() 
        => UnitOfWork.Dispose();
}

