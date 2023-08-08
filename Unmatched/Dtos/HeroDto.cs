namespace Unmatched.Dtos;

public class HeroDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Hp { get; set; }
    
    public int DeckSize { get; set; }
    
    public IEnumerable<SidekickDto> Sidekicks { get; set; }
}