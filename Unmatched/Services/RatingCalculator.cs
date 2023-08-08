using Unmatched.Entities;
using Unmatched.Repositories;

namespace Unmatched.Services;

public class RatingCalculator : IRatingCalculator
{
    private readonly IRatingRepository _ratingRepository;

    public RatingCalculator(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }
    public async Task CalculateAsync(Fighter fighter, Fighter opponent, Tournament tournament)
    {
        var fighterOldRating = await _ratingRepository.GetByHeroIdAsync(fighter.HeroId, tournament.Id);
        var opponentOldRating = await _ratingRepository.GetByHeroIdAsync(opponent.HeroId, tournament.Id);

        var fighterNewPoints = CalculatePoints();
        var opponentNewPoints = CalculatePoints();
        
        var fighterNewRating = new Rating
        {
            TournamentId = tournament.Id,
            HeroId = fighter.HeroId,
            Points = fighterNewPoints
        };
        var opponentNewRating = new Rating
        {
            TournamentId = tournament.Id,
            HeroId = opponent.HeroId,
            Points = opponentNewPoints
        };

        await _ratingRepository.AddAsync(fighterNewRating);
        await _ratingRepository.AddAsync(opponentNewRating);

        await _ratingRepository.SaveChangesAsync();
    }

    private int CalculatePoints()
    {
        return 0;
    }
}