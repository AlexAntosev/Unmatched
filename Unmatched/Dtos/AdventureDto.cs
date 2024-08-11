namespace Unmatched.Dtos;

using System;

public class AdventureDto
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public Guid Id { get; set; }

    public MapDto? Map { get; set; }

    public Guid MapId { get; set; }
    
    public IEnumerable<FighterDto> Fighters { get; set; }
    
    public VillainDto Villain { get; set; }
    
    public Guid VillainId { get; set; }
    
    public IEnumerable<MinionDto> Minions { get; set; }
    
    public bool IsPlanned { get; set; }
    
    public int? Epic { get; set; }
}
