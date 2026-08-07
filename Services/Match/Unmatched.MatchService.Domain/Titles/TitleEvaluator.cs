namespace Unmatched.MatchService.Domain.Titles;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>
/// Runs every registered <see cref="ITitleRule"/> against a match and reconciles each rule's answer onto
/// the corresponding <see cref="TitleEntity"/>'s holders: a <see cref="TitleExclusivity.Shared"/> rule's
/// qualifiers are added without touching existing holders; a <see cref="TitleExclusivity.Unique"/> rule's
/// answer becomes the entire holder set, transferring the title away from anyone not in it (the fix for
/// the old StreakTitleHandler never removing the previous holder). A rule whose Title row hasn't been
/// seeded yet (RuleKey not found) is silently skipped rather than treated as an error, so new rules can
/// ship ahead of their seed migration running in a given environment.
/// </summary>
public class TitleEvaluator(IUnitOfWork unitOfWork, IEnumerable<ITitleRule> rules)
{
    /// <summary>Returns one <see cref="EarnedTitle"/> per hero newly qualifying this match - unlike
    /// <see cref="Title.Holders"/> (the whole current holder set), this is attributed to the specific
    /// fighter who just earned it, for the match-saved result screen.</summary>
    public async Task<List<EarnedTitle>> EvaluateAsync(MatchEntity match)
    {
        var newlyEarned = new List<EarnedTitle>();

        foreach (var rule in rules)
        {
            var title = await unitOfWork.Titles.GetByRuleKeyAsync(rule.RuleKey);
            if (title is null)
            {
                continue;
            }

            var results = await rule.EvaluateAsync(match);
            var holderIds = results.Keys.ToHashSet();
            var existingHolderIds = title.HeroTitles.Select(h => h.HeroesId).ToHashSet();

            if (rule.Exclusivity == TitleExclusivity.Unique)
            {
                foreach (var stale in title.HeroTitles.Where(h => !holderIds.Contains(h.HeroesId)).ToList())
                {
                    title.HeroTitles.Remove(stale);
                }

                // A Unique rule recomputes the current record from the whole history every time, so its
                // metric (e.g. the longest streak so far) needs refreshing even for a holder who isn't
                // new - "holds the title" and "holds the current-best number" are the same fact here.
                foreach (var current in title.HeroTitles.Where(h => holderIds.Contains(h.HeroesId)))
                {
                    current.Metric = results[current.HeroesId];
                }
            }
            else
            {
                // A Shared title's existing holders re-qualifying from this match aren't new holders,
                // but the match still counts toward how many times they've earned it, and the metric
                // (e.g. HP left on this particular win) refreshes to this match's value.
                foreach (var reQualified in title.HeroTitles.Where(h => holderIds.Contains(h.HeroesId)))
                {
                    reQualified.TimesEarned++;
                    reQualified.EarnedAt = match.Date;
                    reQualified.Metric = results[reQualified.HeroesId];
                }
            }

            var newHolderIds = holderIds.Where(id => !existingHolderIds.Contains(id)).ToList();
            foreach (var heroId in newHolderIds)
            {
                title.HeroTitles.Add(new HeroTitleEntity { HeroesId = heroId, TitlesId = title.Id, EarnedAt = match.Date, Metric = results[heroId] });
                newlyEarned.Add(new EarnedTitle { HeroId = heroId, RuleKey = rule.RuleKey, Name = title.Name, Metric = results[heroId] });
            }
        }

        await unitOfWork.SaveChangesAsync();

        return newlyEarned;
    }
}
