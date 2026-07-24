namespace Unmatched.Dtos;

public class UiHeroDto
{
    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }
    
    public bool IsRanged { get; set; }

    public string Name { get; set; }

    public IEnumerable<UiSidekickDto> Sidekicks { get; set; }
    
    public string Color { get; set; }

    public string? ImageFileName { get; set; }

    public string ImageUrl => ImageFileName != null ? $"/images/heroes/{ImageFileName}" : "/Unknown.png";

    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";
    
    public UiPlayStyleDto PlayStyle { get; set; }
}
