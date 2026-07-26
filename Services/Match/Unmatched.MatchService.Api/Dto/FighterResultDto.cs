namespace Unmatched.MatchService.Api.Dto;

public class FighterResultDto
{
    public string HeroName { get; set; }

    public string PlayerName { get; set; }

    public int MatchPoints { get; set; }

    public bool IsWinner { get; set; }

    public int? Placement { get; set; }

    public int? Team { get; set; }
}
