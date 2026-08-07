namespace Unmatched.Dtos.Match;

using Unmatched.Enums;

public class SaveMatchResultDto
{
    public GameMode GameMode { get; set; }

    public List<FighterResultDto> FighterResults { get; set; }

    public bool? PlayersWon { get; set; }

    public string? VillainName { get; set; }
}
