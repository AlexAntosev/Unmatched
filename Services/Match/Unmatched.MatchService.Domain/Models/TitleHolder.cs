namespace Unmatched.MatchService.Domain.Models;

using System;

public class TitleHolder
{
    public Guid HeroId { get; set; }

    public DateTime? EarnedAt { get; set; }

    public int TimesEarned { get; set; }

    public double? Metric { get; set; }
}
