namespace Unmatched.Dtos;

public class UiMatchVillainDto
{
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }

    public VillainDto? Villain { get; set; }

    public Guid VillainId { get; set; }

    public string? Name { get; set; }

    public int? HpLeft { get; set; }

    public int? CardsLeft { get; set; }

    public bool IsWinner { get; set; }

    public IEnumerable<UiMatchMinionDto> Minions { get; set; } = new List<UiMatchMinionDto>();

    /// <summary>Fills in the villain's co-op HP total for the given number of heroes - see
    /// <see cref="VillainDto.EffectiveHp"/>.</summary>
    public void SetDefaultData(int heroCount)
    {
        if (Villain is not null)
        {
            VillainId = Villain.Id;
            HpLeft = Villain.EffectiveHp(heroCount);
            CardsLeft = Villain.DeckSize;
        }
    }
}
