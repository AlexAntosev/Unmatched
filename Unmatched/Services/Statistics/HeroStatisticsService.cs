namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class HeroStatisticsService(IMapper mapper,IMatchClient matchClient, IStatisticsClient statisticsClient, ICatalogClient catalogClient) : IHeroStatisticsService
{
    public async Task<IEnumerable<UiHeroStatisticsDto>> GetHeroesStatisticsAsync()
    {
        var heroStats = await statisticsClient.GetHeroStatsAsync();
        var uiModels = heroStats.Select(mapper.Map<UiHeroStatisticsDto>);
        return uiModels;
    }

    public async Task<UiHeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId)
    {
        var heroStats = await statisticsClient.GetHeroStatsAsync(heroId);
        var uiModel = mapper.Map<UiHeroStatisticsDto>(heroStats);

        var catalogHeroSidekicks = await catalogClient.GetSidekicksByHeroAsync(heroId);
        uiModel.Sidekicks = catalogHeroSidekicks.Select(mapper.Map<UiSidekickDto>);

        uiModel.Titles = new List<TitleDto>(); //TODO

        var catalogPlayStyle = await catalogClient.GetPlayStyleByHero(heroId);
        uiModel.PlayStyle = mapper.Map<UiPlayStyleDto>(catalogPlayStyle);

        return uiModel;
    }

    public async Task<IEnumerable<UiMatchLogDto>> GetHeroMatchesAsync(Guid heroId)
    {
        var matches = await matchClient.GetFinishedByHeroAsync(heroId);
        return matches.Select(mapper.Map<UiMatchLogDto>);
    }

    public async Task<List<RatingChangeDto>> GetRatingChangesAsync(Guid heroId)
    {
        var matches = await matchClient.GetHeroRatingChangesAsync(heroId);
        return matches.Select(mapper.Map<RatingChangeDto>).ToList();
    }
}
