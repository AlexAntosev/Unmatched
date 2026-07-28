namespace Unmatched.StatisticsService.Domain.Models;

/// <summary>
/// How much an expansion actually gets played, aggregated from the stats of the heroes that ship
/// in it. Note this counts hero appearances, not matches: a 2v2 game with two heroes from the same
/// box contributes twice.
/// </summary>
public class ExpansionStats
{
    public Guid ExpansionId { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public int TotalLooses { get; set; }

    public Guid? BestHeroId { get; set; }

    public string? BestHeroName { get; set; }

    public int WinRate
        => TotalMatches > 0
            ? (int)Math.Round((double)TotalWins / TotalMatches * 100)
            : 0;
}
