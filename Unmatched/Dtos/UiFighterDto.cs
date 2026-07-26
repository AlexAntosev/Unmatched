namespace Unmatched.Dtos;

public class UiFighterDto
{
    public int? ActionsMade { get; set; }

    public int? CardsLeft { get; set; }

    public UiHeroDto? Hero { get; set; }

    public Guid HeroId { get; set; } // TODO: get rid of this Id

    // Match service's FighterHeroDto is its own denormalized snapshot of hero data and doesn't carry
    // ImageFileName (unlike CatalogHeroDto) - fall back to the legacy name-based guess for match
    // records until Match service is extended the same way Statistics was. See ImageFileName work
    // in Catalog/Statistics for the pattern to follow.
    public string HeroImageUrl => Hero?.ImageFileName != null ? Hero.ImageUrl : $"/{Hero?.Name ?? "Unknown"}.png";

    public int? HpLeft { get; set; }

    public Guid Id { get; set; }

    public bool IsWinner { get; set; }

    public int? ItemsUsed { get; set; }

    public Guid MatchId { get; set; }

    public int? MatchPoints { get; set; }

    public UiPlayerDto? Player { get; set; }

    public Guid PlayerId { get; set; } // TODO: get rid of this Id

    public string PlayerImageUrl => $"/{Player?.Name ?? "Unknown"}.png";

    public int? SidekickHpLeft { get; set; }

    public string? SidekickName
    {
        get => Hero?.Sidekicks.FirstOrDefault()?.Name;
        set
        {
        }
    }

    public int? TimeSpentInSeconds { get; set; }

    public int? Turn { get; set; }

    public int? Team { get; set; }

    public int? Placement { get; set; }

    public void SetDefaultData()
    {
        if (Hero is not null)
        {
            HpLeft = Hero.Hp;
            HeroId = Hero.Id;
            SidekickHpLeft = Hero.Sidekicks.Sum(s => s.Hp * s.Count);
            CardsLeft = Hero.DeckSize;
            ActionsMade = null;
            TimeSpentInSeconds = null;
        }
    }
}
