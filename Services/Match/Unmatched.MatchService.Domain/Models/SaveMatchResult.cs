namespace Unmatched.MatchService.Domain.Models;

using Unmatched.MatchService.Domain.Enums;

public class SaveMatchResult
{
    public GameMode GameMode { get; set; }

    public List<FighterResult> FighterResults { get; set; }

    public bool? PlayersWon { get; set; }

    public string? VillainName { get; set; }
}
