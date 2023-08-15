namespace Unmatched.Dtos;

public class RankedRatingHeroStatisticsDto : HeroStatisticsDto, IComparable<RankedRatingHeroStatisticsDto>
{
    public int Points { get; set; }
    
    public int CompareTo(RankedRatingHeroStatisticsDto? other)
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