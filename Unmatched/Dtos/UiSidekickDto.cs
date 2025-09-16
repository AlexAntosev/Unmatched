namespace Unmatched.Dtos;

public class UiSidekickDto
{
    public int Hp { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public int Count { get; set; }
    
    public bool IsRanged { get; set; }
    
    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";
}
