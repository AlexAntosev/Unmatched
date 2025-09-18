namespace Unmatched.StatisticsService.Domain.Services;

using AutoMapper;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.Domain.Services.Contracts;

public class HeroStatisticsService(IMapper mapper, ICatalogHeroCache catalogHeroCache, IMatchClient matchClient, IUnitOfWork unitOfWork) : IHeroStatisticsService
{
    public async Task<IEnumerable<HeroStats>> GetHeroesStatisticsAsync()
    {
        var statistics = await unitOfWork.HeroStats.GetAllAsync();

        return statistics;
    }

    public async Task<HeroStats> GetHeroStatisticsAsync(Guid heroId)
    {
        var statistics = await unitOfWork.HeroStats.GetByHeroAsync(heroId);

        return statistics;
    }
}