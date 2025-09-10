namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Repositories;

public class RatingService(IMatchService matchService, IUnitOfWork unitOfWork, IMapper mapper) : IRatingService
{
    public async Task RecalculateAsync()
    {
        var matches = await GetMatchesAsync();
        
        await ClearDataAsync();
        
        foreach (var match in matches)
        {
            await matchService.AddOrUpdateAsync(match);
        }
    }
    
    private async Task<IEnumerable<Match>> GetMatchesAsync()
    {
        var matches = await unitOfWork.Matches.GetAsync();
        return matches.Select(mapper.Map<Match>);
    }

    private async Task ClearDataAsync()
    {
        unitOfWork.Matches.DeleteAll();
        unitOfWork.Fighters.DeleteAll();
        unitOfWork.Ratings.DeleteAll();

        await unitOfWork.SaveChangesAsync();
    }
}
