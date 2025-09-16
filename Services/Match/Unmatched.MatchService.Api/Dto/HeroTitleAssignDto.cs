namespace Unmatched.MatchService.Api.Dto;

using System;

public class HeroTitleAssignDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool IsAssigned { get; set; }
}
