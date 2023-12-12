namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;

public class GoldenHalatLeagueMatchHandler : BaseMatchHandler
{
    private readonly IRatingCalculator _ratingCalculator;
    private readonly IMatchRepository _matchRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IFighterRepository _fighterRepository;

    public GoldenHalatLeagueMatchHandler(
        IRatingCalculator ratingCalculator,
        IMatchRepository matchRepository,
        IRatingRepository ratingRepository,
        IFighterRepository fighterRepository)
    {
        _ratingCalculator = ratingCalculator;
        _matchRepository = matchRepository;
        _ratingRepository = ratingRepository;
        _fighterRepository = fighterRepository;
    }
    
    protected override async Task InnerHandleAsync(Match match)
    {
        var createdMatch = await _matchRepository.AddAsync(match);

        var matchPoints = await _ratingCalculator.CalculateAsync(createdMatch.Fighters.First(), createdMatch.Fighters.Last());

        foreach (var fighter in createdMatch.Fighters)
        {
            UpdateFighterMatchPoints(fighter, matchPoints);
        }

        foreach (var heroMatchPoints in matchPoints)
        {
            await UpdateHeroRatingAsync(heroMatchPoints);
        }

        await _matchRepository.SaveChangesAsync();
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
