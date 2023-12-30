namespace Unmatched.Dtos;

public class HeroStatisticsDto : IComparable<HeroStatisticsDto>
{
    public HeroDto Hero;
    public Guid HeroId { get; set; }
    public string HeroName { get; set; }

    public double Kd
        => TotalMatches > 0
            ? Math.Round((double)TotalWins / TotalMatches, 2)
            : 0;

    public int LastMatchPoints { get; set; }
    
    public int Points { get; set; }

    public int Place { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }
    
    public bool IsRanged { get; set; }
    
    public IEnumerable<TitleDto> Titles { get; set; }

    public int CompareTo(HeroStatisticsDto? other)
    {
        if (other == null)
        {
            return 0;
        }
        
        if (Points != other.Points)
        {
            return Points > other.Points
                ? 1
                : -1;
        }

        if (Kd != other.Kd)
        {
            return Kd > other.Kd
                ? 1
                : -1;
        }
        
        if (TotalMatches != other.TotalMatches)
        {
            return TotalMatches > other.TotalMatches
                ? 1
                : -1;
        }

        return HeroName.CompareTo(other.HeroName);

    }
}
