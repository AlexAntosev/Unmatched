namespace Unmatched.Dtos;

using System;

using Unmatched.Enums;

public class TitleDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string Comment { get; set; }

    public TitleExclusivity Exclusivity { get; set; }

    public string? RuleKey { get; set; }

    public Guid? TournamentId { get; set; }

    public IEnumerable<TitleHolderDto> Holders { get; set; } = [];
}
