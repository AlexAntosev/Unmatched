namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;

public abstract class BaseMatchHandler : IMatchHandler
{
    public void Handle(Match match)
    {
        var fighters = match.Fighters;
        if (IsNotEnoughFighters(fighters))
        {
            throw new ArgumentException("NOT ENOUGH FIGHTERS!");
        }

        InnerHandle(match);
    }

    protected abstract void InnerHandle(Match match);

    private static bool IsNotEnoughFighters(ICollection<Fighter>? fighters) => fighters is null || fighters.Count < 2;
}
