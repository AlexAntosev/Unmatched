namespace Unmatched.MatchService.Domain.Models;

using System;

using Unmatched.MatchService.Domain.Enums;

public class Title
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string Comment { get; set; }

    public TitleExclusivity Exclusivity { get; set; }

    public string? RuleKey { get; set; }

    public Guid? TournamentId { get; set; }

    public IEnumerable<TitleHolder> Holders { get; set; } = [];
}
