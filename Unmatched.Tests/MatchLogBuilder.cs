namespace Unmatched.Tests;

using Unmatched.Dtos;
using Unmatched.Enums;

/// <summary>Terse construction of match log entries so the tests read as scenarios, not setup.</summary>
internal static class MatchLogBuilder
{
    public static UiHeroDto Hero(string name, Guid? id = null)
        => new() { Id = id ?? Guid.NewGuid(), Name = name, Color = "#fff", Sidekicks = [] };

    public static UiPlayerDto Player(string name, Guid? id = null)
        => new() { Id = id ?? Guid.NewGuid(), Name = name };

    public static UiFighterDto Fighter(UiHeroDto hero, bool isWinner, UiPlayerDto? player = null, int? team = null, int? placement = null)
        => new()
            {
                Hero = hero,
                HeroId = hero.Id,
                Player = player,
                PlayerId = player?.Id ?? Guid.Empty,
                IsWinner = isWinner,
                Team = team,
                Placement = placement
            };

    public static UiMatchLogDto Match(
        DateTime date,
        IEnumerable<UiFighterDto> fighters,
        GameMode mode = GameMode.OneVsOne,
        int? epic = null,
        string map = "Castle",
        string tournament = "Golden Halat League")
        => new()
            {
                MatchId = Guid.NewGuid(),
                Date = date,
                Fighters = fighters.ToList(),
                GameMode = mode,
                Epic = epic,
                MapName = map,
                TournamentName = tournament,
                Comment = string.Empty
            };

    /// <summary>A one-on-one where the first hero wins or loses.</summary>
    public static UiMatchLogDto Duel(DateTime date, UiHeroDto hero, UiHeroDto opponent, bool heroWon, int? epic = null)
        => Match(date, [Fighter(hero, heroWon), Fighter(opponent, !heroWon)], epic: epic);
}
