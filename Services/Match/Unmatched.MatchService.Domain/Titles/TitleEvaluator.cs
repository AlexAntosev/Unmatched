namespace Unmatched.MatchService.Domain.Titles;

using AutoMapper;

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
public class TitleEvaluator(IUnitOfWork unitOfWork, IMapper mapper, IEnumerable<ITitleRule> rules)
{
    public async Task<List<Title>> EvaluateAsync(MatchEntity match)
    {
        var newlyEarned = new List<Title>();

        foreach (var rule in rules)
        {
            var title = await unitOfWork.Titles.GetByRuleKeyAsync(rule.RuleKey);
            if (title is null)
            {
                continue;
            }

            var holderIds = await rule.EvaluateAsync(match);
            var existingHolderIds = title.HeroTitles.Select(h => h.HeroesId).ToHashSet();

            if (rule.Exclusivity == TitleExclusivity.Unique)
            {
                foreach (var stale in title.HeroTitles.Where(h => !holderIds.Contains(h.HeroesId)).ToList())
                {
                    title.HeroTitles.Remove(stale);
                }
            }

            var newHolderIds = holderIds.Where(id => !existingHolderIds.Contains(id)).ToList();
            foreach (var heroId in newHolderIds)
            {
                title.HeroTitles.Add(new HeroTitleEntity { HeroesId = heroId, TitlesId = title.Id, EarnedAt = match.Date });
            }

            if (newHolderIds.Count > 0)
            {
                newlyEarned.Add(mapper.Map<Title>(title));
            }
        }

        await unitOfWork.SaveChangesAsync();

        return newlyEarned;
    }
}
