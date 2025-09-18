namespace Unmatched.StatisticsService.Domain.Initialize;

using AutoMapper;

using Microsoft.Extensions.Logging;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Repositories;

public class StatsCoordinatorProvider(
    ILoggerFactory loggerFactory,
    IMapper mapper,
    ICatalogHeroCache catalogHeroCache,
    IMatchClient matchClient,
    IHeroPlaceAdjuster heroPlaceAdjuster) : IStatsCoordinatorProvider
{
    public IEnumerable<IStatsCoordinator> GetAll(IUnitOfWork unitOfWork)
    {
        return new List<IStatsCoordinator>
            {
                new HeroStatsCoordinator(loggerFactory, mapper, unitOfWork, catalogHeroCache, matchClient, heroPlaceAdjuster)
            };
    }
}
