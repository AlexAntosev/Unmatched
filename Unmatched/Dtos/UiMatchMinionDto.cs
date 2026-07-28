namespace Unmatched.Dtos;

public class UiMatchMinionDto
{
    public Guid Id { get; set; }

    public Guid MatchVillainId { get; set; }

    public MinionDto? Minion { get; set; }

    public Guid MinionId { get; set; }

    public string? Name { get; set; }

    public int? HpLeft { get; set; }

    public bool IsWinner { get; set; }

    public void SetDefaultData()
    {
        if (Minion is not null)
        {
            MinionId = Minion.Id;
            HpLeft = Minion.Hp;
        }
    }
}
