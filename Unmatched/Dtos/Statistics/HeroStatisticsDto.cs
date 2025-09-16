namespace Unmatched.Dtos.Statistics;

public class HeroStatisticsDto
{
    public string Color { get; set; }

    public int DeckSize { get; set; }

    public Guid HeroId { get; set; }

    public int Hp { get; set; }

    public bool IsRanged { get; set; }

    public double Kd { get; set; }

    public int LastMatchPoints { get; set; }

    public string Name { get; set; }

    public int Place { get; set; }

    public int Points { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }
}
