namespace Unmatched.MatchService.Domain.Models;

public class FighterHero
{
    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<FighterSidekick> Sidekicks { get; set; }

    public string Color { get; set; }
}
