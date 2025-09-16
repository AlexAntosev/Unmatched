namespace Unmatched.Dtos;

public class PlayerStatisticsDto : IComparable<PlayerStatisticsDto>
{
    public string ImageUrl => $"/{Name}.png";

    public double Kd { get; set; }

    public int LastMatchPoints { get; set; }

    public string Name { get; set; }

    public int Place { get; set; }

    public Guid PlayerId { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public int CompareTo(PlayerStatisticsDto? other)
    {
        if (other == null)
        {
            return 0;
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

        return Name.CompareTo(other.Name);
    }
}
