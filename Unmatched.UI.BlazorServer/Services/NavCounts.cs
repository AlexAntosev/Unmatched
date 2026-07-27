namespace Unmatched.UI.BlazorServer.Services;

/// <summary>The trailing figures shown next to each nav rail item.</summary>
public record NavCounts(
    int Heroes,
    int Players,
    int Maps,
    int Minions,
    int Villains,
    int Matches,
    int OwnedExpansions,
    int TotalExpansions,
    int Titles,
    int Tournaments)
{
    public static readonly NavCounts Empty = new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

    public string Collection => $"{OwnedExpansions}/{TotalExpansions}";
}
