namespace Unmatched.MatchService.Domain.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Unmatched.MatchService.Domain.Enums;

[Table("Titles")]
public class TitleEntity
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }

    [ForeignKey(nameof(HeroTitleEntity.TitlesId))]
    public virtual ICollection<HeroTitleEntity> HeroTitles { get; set; }

    public string Comment { get; set; }

    public TitleExclusivity Exclusivity { get; set; }

    /// <summary>Set for a rule-backed title (see Domain/Titles/ITitleRule) - null for a manually
    /// created one. Replaces the old GetByNameAsync(name) magic-string lookup: a title's source is
    /// derivable (RuleKey != null → automatic, TournamentId != null → tournament, else manual).</summary>
    public string? RuleKey { get; set; }

    /// <summary>Set for a per-tournament title (Champion, RunnerUp, ...) - each tournament gets its own
    /// copy, so two tournaments' Champions coexist as separate rows instead of sharing one holder set.</summary>
    public Guid? TournamentId { get; set; }
}