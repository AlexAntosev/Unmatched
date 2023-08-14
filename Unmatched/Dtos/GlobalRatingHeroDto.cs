namespace Unmatched.Dtos;

public class GlobalRatingHeroDto : IComparable<GlobalRatingHeroDto>
{
    public int Place { get; set; }
    public int Points { get; set; }
    public int TotalMatches { get; set; }
    public int TotalWins { get; set; }
    public int TotalLooses { get; set; }
    public int LastMatchPoints { get; set; }

    public double Kd => TotalMatches > 0 ? Math.Round((double)TotalWins / TotalMatches, 2) : 0;

    public string HeroName { get; set; }

    public int CompareTo(GlobalRatingHeroDto? other)
    {
        if (other == null) return 0;

        if (Points != other.Points) return Points > other.Points ? 1 : -1;

        if (Points != other.Points) return Points > other.Points ? 1 : -1;

        return 0;
    }
}