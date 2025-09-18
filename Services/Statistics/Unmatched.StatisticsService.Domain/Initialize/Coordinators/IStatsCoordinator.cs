namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;

public interface IStatsCoordinator
{
    Task<bool> HasDataAsync();

    Task InitializeAsync(IReadOnlyCollection<MatchDto> matches);

    Task CheckAndInitializeAsync();
}