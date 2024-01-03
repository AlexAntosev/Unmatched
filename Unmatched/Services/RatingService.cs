namespace Unmatched.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Entities;
using Unmatched.Repositories;

public class RatingService : IRatingService
{
    private readonly IMatchService _matchService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public RatingService(IMatchService matchService, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _matchService = matchService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task RecalculateAsync()
    {
        var matches = GetMatches();
        
        await ClearDataAsync();
        
        foreach (var match in matches)
        {
            await _matchService.AddAsync(match);
        }
    }
    
    private IEnumerable<Match> GetMatches()
    {
        var matches = _unitOfWork.Matches.Query().Include(m => m.Fighters).OrderBy(x => x.Date).AsNoTracking().ToArray();
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
