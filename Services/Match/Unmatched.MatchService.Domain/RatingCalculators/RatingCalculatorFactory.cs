namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Selects the calculator for a ranked match by <see cref="GameMode"/> alone - tournament
/// dispatch is gone, see MatchHandler for the IsRanked gate. Mirrors
/// <see cref="Validation.GameModeValidatorFactory"/>: the calculators are stateless besides the
/// shared unit of work and hero cache, so they're constructed directly rather than resolved from
/// the container.</summary>
public class RatingCalculatorFactory(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache) : IRatingCalculatorFactory
{
    public IRatingCalculator Create(GameMode gameMode) => gameMode switch
    {
        GameMode.OneVsOne => new OneVsOneRatingCalculator(unitOfWork, catalogHeroCache),
        GameMode.TeamVsTeam => new TeamVsTeamRatingCalculator(unitOfWork, catalogHeroCache),
        GameMode.FreeForAll => new FreeForAllRatingCalculator(unitOfWork, catalogHeroCache),
        GameMode.Cooperative => new NoRatingCalculator(),
        _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, "Unknown game mode.")
    };
}
