namespace Unmatched.Services;

using Unmatched.DataInitialization;
using Unmatched.Repositories;

public class FirstTournamentRatingCalculator : IFirstTournamentRatingCalculator
{
    private readonly IHeroRepository _heroRepository;

    public FirstTournamentRatingCalculator(IHeroRepository heroRepository)
    {
        _heroRepository = heroRepository;
    }

    public async Task<IEnumerable<HeroMatchPoints>> CalculateAsync(FirstTournamentMatchInfo matchInfo)
    {
        var winnerHeroId = matchInfo.AndriiHp > 0
            ? matchInfo.AndriiHeroId
            : matchInfo.OlexHeroId;
        
        var winnerHp = matchInfo.AndriiHp > 0
            ? matchInfo.AndriiHp
            : matchInfo.OlexHp;
        
        var looserHeroId = matchInfo.OlexHp > 0
            ? matchInfo.AndriiHeroId
            : matchInfo.OlexHeroId;

        var winnerHeroMaxHp = (await _heroRepository.GetByIdAsync(winnerHeroId)).Hp;
        
        var coeficient = matchInfo.MatchLevel switch
            {
                MatchLevel.Group => 2,
                MatchLevel.QuarterFinals => 8,
                MatchLevel.SemiFinals => 8,
                MatchLevel.ThirdPlaceFinals => 8,
                MatchLevel.Finals => 4,
            };

        var winnerPoints = Convert.ToInt32(Math.Round(coeficient * (80 + 40 * ((double)winnerHp / winnerHeroMaxHp)), 0));

        return new List<HeroMatchPoints>()
            {
                new()
                    {
                        HeroId = looserHeroId,
                        Points = 0
                    },
                new()
                    {
                        HeroId = winnerHeroId,
                        Points = winnerPoints
                    },
            };
    }
}
