namespace Unmatched.Dtos;

public class UiHeroStatisticsDto : IComparable<UiHeroStatisticsDto>
{
    public Guid HeroId { get; set; }

    public string ImageUrl => $"/{Name}.png";

    public bool IsRanged { get; set; }

    public double Kd { get; set; }

    public int LastMatchPoints { get; set; }

    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";

    public string Name { get; set; }

    public IEnumerable<UiSidekickDto>? Sidekicks { get; set; }

    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public UiPlayStyleDto? PlayStyle { get; set; }

    public string Color { get; set; }

    public int Place { get; set; }

    public int Points { get; set; }

    public IEnumerable<TitleDto>? Titles { get; set; }

    public int TotalLooses { get; set; }

    public int TotalMatches { get; set; }

    public int TotalWins { get; set; }

    public int CompareTo(UiHeroStatisticsDto? other)
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

        return Name.CompareTo(other.Name);
    }
}
