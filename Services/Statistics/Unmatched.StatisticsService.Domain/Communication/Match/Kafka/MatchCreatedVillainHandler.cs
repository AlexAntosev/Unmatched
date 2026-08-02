namespace Unmatched.StatisticsService.Domain.Communication.Match.Kafka;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;

public class MatchCreatedVillainHandler(ICatalogVillainCache catalogVillainCache) : IMatchCreatedHandler
{
    public async Task HandleAsync(IUnitOfWork unitOfWork, MatchCreated matchCreated)
    {
        var villain = matchCreated.Villain;
        if (villain is null)
        {
            return;
        }

        var villainStats = await unitOfWork.VillainStats.GetByVillainAsync(villain.VillainId);
        if (villainStats is null)
        {
            var catalogVillain = await catalogVillainCache.GetAsync(villain.VillainId);
            villainStats = new VillainStats
                {
                    VillainId = villain.VillainId,
                    Name = catalogVillain?.Name ?? villain.Name ?? string.Empty,
                    Color = catalogVillain?.Color ?? string.Empty,
                    BaseHp = catalogVillain?.BaseHp ?? 0,
                    HpPerExtraPlayer = catalogVillain?.HpPerExtraPlayer ?? 0,
                    DeckSize = catalogVillain?.DeckSize ?? 0,
                    IsRanged = catalogVillain?.IsRanged ?? false,
                    ImageFileName = catalogVillain?.ImageFileName
                };
        }

        villainStats.LastMatchIncludedAt = matchCreated.Date;
        villainStats.ModifiedAt = DateTime.UtcNow;
        villainStats.TotalMatches++;
        if (villain.IsWinner)
        {
            villainStats.TotalWins++;
        }
        else
        {
            villainStats.TotalLooses++;
        }

        await unitOfWork.VillainStats.AddOrUpdateAsync(villainStats);
    }
}
