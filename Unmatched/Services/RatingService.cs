namespace Unmatched.Services;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;

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
    
    private async Task<IEnumerable<Match>> GetMatchesAsync()
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
