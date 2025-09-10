namespace Unmatched.MatchService.Domain.Dto;

using System;

public class HeroTitleAssign
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool IsAssigned { get; set; }
}
