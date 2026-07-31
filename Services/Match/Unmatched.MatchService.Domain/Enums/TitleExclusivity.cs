namespace Unmatched.MatchService.Domain.Enums;

/// <summary>Shared titles can have many simultaneous holders (e.g. Rusher). Unique titles have exactly
/// one holder at a time - earning it transfers it away from whoever held it before (e.g. The Streak).</summary>
public enum TitleExclusivity
{
    Shared,
    Unique
}
