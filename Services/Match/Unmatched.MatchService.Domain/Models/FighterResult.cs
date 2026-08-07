namespace Unmatched.MatchService.Domain.Models;

public class FighterResult
{
    public Guid HeroId { get; set; }

    public string HeroName { get; set; }

    public string PlayerName { get; set; }

    public int MatchPoints { get; set; }

    public bool IsWinner { get; set; }

    public int? Placement { get; set; }

    public int? Team { get; set; }

    public int? RatingBefore { get; set; }

    public int? RatingAfter { get; set; }

    public List<EarnedTitle> EarnedTitles { get; set; } = [];
}
