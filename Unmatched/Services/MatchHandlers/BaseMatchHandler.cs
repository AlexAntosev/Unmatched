namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;

public abstract class BaseMatchHandler : IMatchHandler
{
    public async Task HandleAsync(Match match)
    {
        var fighters = match.Fighters;
        if (IsNotEnoughFighters(fighters))
        {
            throw new ArgumentException("NOT ENOUGH FIGHTERS!");
        }

        await InnerHandleAsync(match);
    }

    protected abstract Task InnerHandleAsync(Match match);

    protected bool IsNotEnoughFighters(ICollection<Fighter>? fighters) => fighters is null || fighters.Count < 2;
}
