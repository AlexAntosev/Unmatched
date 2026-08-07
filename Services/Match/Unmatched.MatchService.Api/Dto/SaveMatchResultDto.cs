namespace Unmatched.MatchService.Api.Dto;

using Unmatched.MatchService.Domain.Enums;

public class SaveMatchResultDto
{
    public GameMode GameMode { get; set; }

    public List<FighterResultDto> FighterResults { get; set; }

    public bool? PlayersWon { get; set; }

    public string? VillainName { get; set; }
}
