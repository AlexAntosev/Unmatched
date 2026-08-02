namespace Unmatched.UI.BlazorServer.Shared.TurnOrder;

/// <summary>
/// The 2v2 turn-numbering rule (design/IMPLEMENTATION-PROMPT.md §5.4): team 1 gets 1, 3, 5…,
/// team 2 gets 2, 4, 6…, each team's own order independent of the other's. Shared by
/// MatchSheet's drag-confined-to-team renumber/shuffle and the draft session's turn-order roll,
/// so the two can never drift apart on what "team-alternating" means.
/// </summary>
public static class TeamAlternatingTurns
{
    public static int TurnFor(int team, int indexWithinTeam) => (team == 2 ? 2 : 1) + indexWithinTeam * 2;
}
