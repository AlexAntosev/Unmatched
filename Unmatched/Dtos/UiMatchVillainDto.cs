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

    public void SetDefaultData()
    {
        if (Villain is not null)
        {
            VillainId = Villain.Id;
            HpLeft = Villain.Hp;
            CardsLeft = Villain.DeckSize;
        }
    }
}
