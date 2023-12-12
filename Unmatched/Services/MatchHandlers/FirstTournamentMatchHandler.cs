namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;

public class FirstTournamentMatchHandler : BaseMatchHandler
{
    private readonly IFirstTournamentRatingCalculator _ratingCalculator;
    private readonly IMatchRepository _matchRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IFighterRepository _fighterRepository;

    private readonly IMatchStageRepository _matchStageRepository;

    public FirstTournamentMatchHandler(
        IFirstTournamentRatingCalculator ratingCalculator,
        IMatchRepository matchRepository,
        IRatingRepository ratingRepository,
        IFighterRepository fighterRepository,
        IMatchStageRepository matchStageRepository)
    {
        _ratingCalculator = ratingCalculator;
        _matchRepository = matchRepository;
        _ratingRepository = ratingRepository;
        _fighterRepository = fighterRepository;
        _matchStageRepository = matchStageRepository;
    }

    public async Task HandleAsync(Match match, Stage stage)
    {
        var createdMatch = await _matchRepository.AddAsync(match);

        await CreateMatchStage(stage, createdMatch);
        await _matchStageRepository.SaveChangesAsync();
        
        await HandleAsync(createdMatch);
    }

    private async Task CreateMatchStage(Stage stage, Match createdMatch)
    {
        var matchStage = new MatchStage
            {
                MatchId = createdMatch.Id,
                Stage = stage
            };
        await _matchStageRepository.AddAsync(matchStage);
    }

    protected override async Task InnerHandleAsync(Match match)
    {
        var matchStage = await _matchStageRepository.GetByMatchIdAsync(match.Id);
        
        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last(), matchStage.Stage);

        foreach (var fighter in match.Fighters)
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
