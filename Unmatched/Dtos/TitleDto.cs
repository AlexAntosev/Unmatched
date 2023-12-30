namespace Unmatched.Dtos;

using System;

public class TitleDto
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    
    public IEnumerable<HeroDto> Heroes { get; set; }
}
