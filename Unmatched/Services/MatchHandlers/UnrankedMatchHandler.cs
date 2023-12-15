namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class UnrankedMatchHandler : BaseMatchHandler
{
    private readonly IMatchRepository _matchRepository;

    private readonly IFighterRepository _fighterRepository;

    private readonly IUnrankedRatingCalculator _ratingCalculator;

    private readonly IRatingRepository _ratingRepository;

    public UnrankedMatchHandler(IMatchRepository matchRepository, IFighterRepository fighterRepository, IUnrankedRatingCalculator ratingCalculator, IRatingRepository ratingRepository)
    {
        _matchRepository = matchRepository;
        _fighterRepository = fighterRepository;
        _ratingCalculator = ratingCalculator;
        _ratingRepository = ratingRepository;
    }
    protected override async Task InnerHandleAsync(Match match)
    {
        var createdMatch = await _matchRepository.AddAsync(match);
        await _matchRepository.SaveChangesAsync();
        var matchPoints = await _ratingCalculator.CalculateAsync(createdMatch.Fighters.First(), createdMatch.Fighters.Last());

        foreach (var fighter in createdMatch.Fighters)
        {
            UpdateFighterMatchPoints(fighter, matchPoints);
        }

        foreach (var heroMatchPoints in matchPoints)
        {
            await UpdateHeroRatingAsync(heroMatchPoints);
        }


        await _fighterRepository.SaveChangesAsync();
        await _ratingRepository.SaveChangesAsync();
    }
    
    private void UpdateFighterMatchPoints(Fighter fighter, IEnumerable<HeroMatchPoints> matchPoints)
    {
        fighter.MatchPoints = matchPoints.FirstOrDefault(h => h.HeroId == fighter.HeroId).Points;
        _fighterRepository.AddOrUpdate(fighter);
    }

    private async Task UpdateHeroRatingAsync(HeroMatchPoints heroMatchPoints)
    {
        var heroRating = await _ratingRepository.GetByHeroIdAsync(heroMatchPoints.HeroId)
         ?? new Rating
                {
                    HeroId = heroMatchPoints.HeroId
                };
        
        var matchPoints = heroMatchPoints.Points;
        heroRating.Points += matchPoints;
        
        _ratingRepository.AddOrUpdate(heroRating);
    }
}
