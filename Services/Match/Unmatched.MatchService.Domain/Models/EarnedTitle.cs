namespace Unmatched.MatchService.Domain.Models;

/// <summary>A title newly earned by one hero on the match just evaluated - the attributed counterpart
/// to <see cref="Title"/>, which only exposes the full current holder set.</summary>
public class EarnedTitle
{
    public Guid HeroId { get; set; }

    public string RuleKey { get; set; }

    public string Name { get; set; }

    public double? Metric { get; set; }
}
