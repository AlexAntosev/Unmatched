namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;

public interface IExpansionStatisticsService
{
    Task<IReadOnlyDictionary<Guid, UiExpansionStatisticsDto>> GetByExpansionAsync();
}
