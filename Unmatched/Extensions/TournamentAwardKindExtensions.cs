namespace Unmatched.Extensions;

using Unmatched.Enums;

public static class TournamentAwardKindExtensions
{
    public static string GetLabel(this TournamentAwardKind kind)
    {
        return kind switch
            {
                TournamentAwardKind.Winner => "Winner",
                TournamentAwardKind.Finalist => "Finalist",
                TournamentAwardKind.Semifinalist => "Semifinalist",
                TournamentAwardKind.Quarterfinalist => "Quarterfinalist",
                TournamentAwardKind.EighthFinalist => "1/8 Finalist",
                TournamentAwardKind.SixteenthFinalist => "1/16 Finalist",
                TournamentAwardKind.GroupWinner => "Group winner",
                TournamentAwardKind.SeasonFirst => "1st place",
                TournamentAwardKind.SeasonSecond => "2nd place",
                TournamentAwardKind.SeasonThird => "3rd place",
                TournamentAwardKind.Eliminated => "Eliminated",
                _ => ""
            };
    }
}
