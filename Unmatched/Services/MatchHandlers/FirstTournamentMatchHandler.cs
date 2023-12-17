namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class FirstTournamentMatchHandler : BaseMatchHandler
{
    private readonly IFirstTournamentRatingCalculator _ratingCalculator;

    public FirstTournamentMatchHandler(IUnitOfWork unitOfWork, IFirstTournamentRatingCalculator ratingCalculator)
    : base(unitOfWork)
    {
        _ratingCalculator = ratingCalculator;
    }
    
    protected override async Task InnerHandleAsync(Match match)
    {
        if (match is not MatchWithStage matchWithStage)
        {
            throw new InvalidCastException($"{nameof(match)} is not of type {nameof(MatchWithStage)}");
        }
        
        var matchPoints = (await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last(), matchWithStage.Stage)).ToArray();
        
        foreach (var fighter in match.Fighters)
        {
            fighter.MatchPoints = GetFighterMatchPoints(matchPoints, fighter.HeroId);
        }

        var createdMatch = await UnitOfWork.Matches.AddAsync(match);
        
        await CreateMatchStageAsync(matchWithStage.Stage, createdMatch);

        var tasks = matchPoints.Select(UpdateHeroRatingAsync);
        await Task.WhenAll(tasks);
        
        await UnitOfWork.SaveChangesAsync();
    }
    
    private Task<MatchStage> CreateMatchStageAsync(Stage stage, Match createdMatch)
    {
        var matchStage = new MatchStage
            {
                MatchId = createdMatch.Id,
                Stage = stage
            };
        return UnitOfWork.MatchStages.AddAsync(matchStage);
    }

    private static int GetFighterMatchPoints(IEnumerable<HeroMatchPoints> matchPoints, Guid heroId) 
        => matchPoints.FirstOrDefault(h => h.HeroId == heroId).Points;

    private async Task UpdateHeroRatingAsync(HeroMatchPoints heroMatchPoints)
    {
        var heroRating = await UnitOfWork.Ratings.GetByHeroIdAsync(heroMatchPoints.HeroId)
         ?? new Rating
                {
                    HeroId = heroMatchPoints.HeroId
                };
        
        var matchPoints = heroMatchPoints.Points;
        heroRating.Points += matchPoints;
        
        UnitOfWork.Ratings.AddOrUpdate(heroRating);
    }
}
