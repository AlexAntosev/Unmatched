namespace Unmatched.Dtos;

public class HeroDto
{
    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<SidekickDto> Sidekicks { get; set; }
    
    public string Color { get; set; }
    
    public IEnumerable<TitleDto> Titles { get; set; }
}
