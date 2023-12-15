﻿namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

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

    protected override async Task InnerHandleAsync(Match match)
    {
        var matchWithStage = (MatchWithStage)match;
        var createdMatch = await _matchRepository.AddAsync(match);

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

        await _matchRepository.SaveChangesAsync();
        await _matchStageRepository.SaveChangesAsync();
        await _fighterRepository.SaveChangesAsync();
        await _ratingRepository.SaveChangesAsync();
    }
    
    private async Task<MatchStage> CreateMatchStage(Stage stage, Match createdMatch)
    {
        var matchStage = new MatchStage
            {
                MatchId = createdMatch.Id,
                Stage = stage
            };
        return await _matchStageRepository.AddAsync(matchStage);
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
