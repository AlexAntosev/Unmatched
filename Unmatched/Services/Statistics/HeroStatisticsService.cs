namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class HeroStatisticsService(IMapper mapper,IMatchClient matchClient) : IHeroStatisticsService
{
    public Task<IEnumerable<HeroStatisticsDto>> GetHeroesStatisticsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<HeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId)
    {
        throw new NotImplementedException();
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
