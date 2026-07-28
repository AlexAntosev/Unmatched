namespace Unmatched.MatchService.Domain.Validation;

using Unmatched.MatchService.Domain.Entities;

public interface IGameModeValidator
{
    void Validate(MatchEntity match);
}
