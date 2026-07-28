namespace Unmatched.MatchService.Domain.Validation;

using Unmatched.MatchService.Domain.Enums;

public class GameModeValidatorFactory : IGameModeValidatorFactory
{
    public IGameModeValidator Create(GameMode gameMode) => gameMode switch
    {
        GameMode.OneVsOne => new OneVsOneValidator(),
        GameMode.TeamVsTeam => new TeamVsTeamValidator(),
        GameMode.FreeForAll => new FreeForAllValidator(),
        GameMode.Cooperative => new CooperativeValidator(),
        _ => throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, "Unknown game mode.")
    };
}
