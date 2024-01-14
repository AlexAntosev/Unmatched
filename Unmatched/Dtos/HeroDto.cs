namespace Unmatched.Dtos;

public class HeroDto
{
    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }
    
    public bool IsRanged { get; set; }

    public string Name { get; set; }

    public IEnumerable<SidekickDto> Sidekicks { get; set; }
    
    public string Color { get; set; }
    
    public IEnumerable<TitleDto> Titles { get; set; }
    
    public string ImageUrl => $"/{Name}.png";
    
    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";
}
