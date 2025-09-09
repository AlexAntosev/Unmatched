namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class RatingService : IRatingService
{
    private readonly IMatchService _matchService;
    private readonly IUnitOfWork _unitOfWork;

    public RatingService(IMatchService matchService, IUnitOfWork unitOfWork)
    {
        _matchService = matchService;
        _unitOfWork = unitOfWork;
    }
    public async Task RecalculateAsync()
    {
        var matches = await GetMatchesAsync();
        
        await ClearDataAsync();
        
        foreach (var match in matches)
        {
            await _matchService.AddAsync(match);
        }
    }
    
    private async Task<IEnumerable<MatchEntity>> GetMatchesAsync()
    {
        var matches = await _unitOfWork.Matches.GetAsync();
        return matches;
    }

    private async Task ClearDataAsync()
    {
        _unitOfWork.Matches.DeleteAll();
        _unitOfWork.Fighters.DeleteAll();
        _unitOfWork.Ratings.DeleteAll();

        await _unitOfWork.SaveChangesAsync();
    }
}
