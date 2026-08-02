using System.ComponentModel.DataAnnotations.Schema;

namespace Unmatched.MatchService.Domain.Entities;

[Table("HeroTitle")]
public class HeroTitleEntity
{
    public Guid TitlesId { get; set; }

    public Guid HeroesId { get; set; }

    /// <summary>Nullable because existing rows genuinely have no date - they were assigned manually
    /// before this column existed.</summary>
    public DateTime? EarnedAt { get; set; }

    /// <summary>How many times this hero has qualified for a Shared title (an achievement) - bumped by
    /// <see cref="Titles.TitleEvaluator"/> each time an existing holder re-qualifies. Always 1 for a
    /// Unique title's sole holder and for manually assigned titles.</summary>
    public int TimesEarned { get; set; } = 1;

    /// <summary>The rule-specific "how much/how many" behind this title, e.g. total sidekick HP
    /// destroyed for Executioner or HP left for Last Breath - only set for the rules where a single
    /// number is meaningful (see <see cref="Titles.ITitleRule"/>). Null for rules with no such number
    /// (Flawless, Deck Miller, Kingslayer, Bounty Holder) and for manually assigned titles. What the
    /// number means is entirely rule-specific; formatting it for display is the presentation layer's
    /// job, keyed by the title's RuleKey.</summary>
    public double? Metric { get; set; }
}
