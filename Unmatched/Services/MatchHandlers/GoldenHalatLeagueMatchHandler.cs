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

        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last());

        foreach (var fighter in match.Fighters)
        {
            fighter.MatchId = createdMatch.Id;
            fighter.MatchPoints = matchPoints.FirstOrDefault(h => h.HeroId == fighter.HeroId).Points;
            await _fighterRepository.AddAsync(fighter);
        }

        foreach (var heroMatchPoints in matchPoints)
        {
            await UpdateHeroRatingAsync(heroMatchPoints);
        }

        await _matchRepository.SaveChangesAsync();
        await _fighterRepository.SaveChangesAsync();
        await _ratingRepository.SaveChangesAsync();
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
