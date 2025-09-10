namespace Unmatched.MatchService.Domain.Dto;

using System;

public class Title
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    
    public string Comment { get; set; }
}
