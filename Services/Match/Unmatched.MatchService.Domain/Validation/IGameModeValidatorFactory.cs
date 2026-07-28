namespace Unmatched.MatchService.Domain.Validation;

using Unmatched.MatchService.Domain.Enums;

public interface IGameModeValidatorFactory
{
    IGameModeValidator Create(GameMode gameMode);
}
