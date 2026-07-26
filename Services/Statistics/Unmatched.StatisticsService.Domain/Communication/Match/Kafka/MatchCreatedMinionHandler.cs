namespace Unmatched.StatisticsService.Domain.Communication.Match.Kafka;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;

public class MatchCreatedMinionHandler(ICatalogMinionCache catalogMinionCache) : IMatchCreatedHandler
{
    public async Task HandleAsync(IUnitOfWork unitOfWork, MatchCreated matchCreated)
    {
        var minions = matchCreated.Villain?.Minions ?? Enumerable.Empty<MatchCreated.MatchMinion>();
        foreach (var minion in minions)
        {
            var minionStats = await unitOfWork.MinionStats.GetByMinionAsync(minion.MinionId);
            if (minionStats is null)
            {
                var catalogMinion = await catalogMinionCache.GetAsync(minion.MinionId);
                minionStats = new MinionStats
                    {
                        MinionId = minion.MinionId,
                        Name = catalogMinion?.Name ?? minion.Name ?? string.Empty,
                        Color = catalogMinion?.Color ?? string.Empty,
                        Hp = catalogMinion?.Hp ?? 0,
                        DeckSize = catalogMinion?.DeckSize ?? 0,
                        IsRanged = catalogMinion?.IsRanged ?? false,
                        ImageFileName = catalogMinion?.ImageFileName
                    };
            }

            minionStats.LastMatchIncludedAt = matchCreated.Date;
            minionStats.ModifiedAt = DateTime.UtcNow;
            minionStats.TotalMatches++;
            if (minion.IsWinner)
            {
                minionStats.TotalWins++;
            }
            else
            {
                minionStats.TotalLooses++;
            }

            await unitOfWork.MinionStats.AddOrUpdateAsync(minionStats);
        }
    }
}
