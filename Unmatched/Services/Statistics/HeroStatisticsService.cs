namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class HeroStatisticsService(IMapper mapper,IMatchClient matchClient, IStatisticsClient statisticsClient) : IHeroStatisticsService
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

        uiModel.Sidekicks = new List<UiSidekickDto>();
        uiModel.Titles = new List<TitleDto>();
        uiModel.PlayStyle =null;

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
