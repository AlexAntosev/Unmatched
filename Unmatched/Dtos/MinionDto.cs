namespace Unmatched.Dtos;

using System;

public class MinionDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Hp { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }
    
    public string Color { get; set; }
    
    public string ImageUrl => $"/{Name}.png";
    
    public string MeleeRangeImageUrl => $"/{(IsRanged ? "Ranged" : "Melee")}.png";
}
