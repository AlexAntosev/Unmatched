namespace Unmatched.MatchService.Api.Dto;

using System;

using Unmatched.MatchService.Domain.Enums;

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
