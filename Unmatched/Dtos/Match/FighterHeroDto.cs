namespace Unmatched.Dtos.Match;

public class FighterHeroDto
{
    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<FighterSidekickDto> Sidekicks { get; set; }

    public string Color { get; set; }
}
