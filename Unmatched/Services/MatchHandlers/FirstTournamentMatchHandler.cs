namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class FirstTournamentMatchHandler : BaseMatchHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFirstTournamentRatingCalculator _ratingCalculator;

    public FirstTournamentMatchHandler(IUnitOfWork unitOfWork, IFirstTournamentRatingCalculator ratingCalculator)
    {
        _unitOfWork = unitOfWork;
        _ratingCalculator = ratingCalculator;
    }
    
    protected override async Task InnerHandleAsync(Match match)
    {
        var matchWithStage = (MatchWithStage)match;
        var createdMatch = await _unitOfWork.Matches.AddAsync(match);
        await _unitOfWork.SaveChangesAsync();
        
        var matchStage = await CreateMatchStage(matchWithStage.Stage, createdMatch);
        
        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last(), matchStage.Stage);

        foreach (var fighter in match.Fighters)
        {
            UpdateFighterMatchPoints(fighter, matchPoints);
        }

        foreach (var heroMatchPoints in matchPoints)
        {
            await UpdateHeroRatingAsync(heroMatchPoints);
        }
        
        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task<MatchStage> CreateMatchStage(Stage stage, Match createdMatch)
    {
        var matchStage = new MatchStage
            {
                MatchId = createdMatch.Id,
                Stage = stage
            };
        return await _unitOfWork.MatchStages.AddAsync(matchStage);
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
