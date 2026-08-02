namespace Unmatched.MatchService.Domain.Constants;

/// <summary>Rule keys for every automatic title (see Domain/Titles/ITitleRule) - the seeded Title row for
/// each one carries the matching RuleKey, which is how <c>TitleEvaluator</c> finds it without a
/// name-string lookup.</summary>
public static class Titles
{
    // Shared
    public const string Flawless = "flawless";
    public const string LastBreath = "last-breath";
    public const string GiantSlayer = "giant-slayer";
    public const string DeckMiller = "deck-miller";

    // Unique
    public const string Streak = "streak";
    public const string Sufferer = "sufferer";
    public const string GrandChampion = "grand-champion";
    public const string Kingslayer = "kingslayer";
    public const string Executioner = "executioner";
    public const string Wall = "wall";
    public const string Workhorse = "workhorse";
    public const string BountyHolder = "bounty-holder";
}
