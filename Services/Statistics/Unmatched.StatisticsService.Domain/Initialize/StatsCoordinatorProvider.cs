namespace Unmatched.StatisticsService.Domain.Initialize;

using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Repositories;

public class StatsCoordinatorProvider(IEnumerable<IStatsCoordinator> coordinators) : IStatsCoordinatorProvider
{
    public IEnumerable<IStatsCoordinator> GetAll(IUnitOfWork unitOfWork)
    {
        foreach (var coordinator in coordinators)
        {
            coordinator.UnitOfWork = unitOfWork;
        }

        return coordinators;
    }
}
