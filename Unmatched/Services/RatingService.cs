namespace Unmatched.Services;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Unmatched.Entities;
using Unmatched.Repositories;

public class RatingService : IRatingService
{
    private readonly IMatchService _matchService;
    private readonly IMatchRepository _matchRepository;
    private readonly IFighterRepository _fighterRepository;

    private readonly IMatchStageRepository _matchStageRepository;

    private readonly IMapper _mapper;

    private readonly IRatingRepository _ratingRepository;

    public RatingService(IMatchService matchService, IMatchRepository matchRepository, IFighterRepository fighterRepository, IMatchStageRepository matchStageRepository, IMapper mapper, IRatingRepository ratingRepository)
    {
        _matchService = matchService;
        _matchRepository = matchRepository;
        _fighterRepository = fighterRepository;
        _matchStageRepository = matchStageRepository;
        _mapper = mapper;
        _ratingRepository = ratingRepository;
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
        var matches = _matchRepository.Query().Include(m => m.Fighters).AsNoTracking().ToArray();
        var matchStages = _matchStageRepository.Query().ToArray();
        foreach (var matchStage in matchStages)
        {
            var match = matches.First(x => x.Id.Equals(matchStage.MatchId));
            var matchWithStage = _mapper.Map<MatchWithStage>(match);
            matchWithStage.Stage = matchStage.Stage;
            var matchIndex = Array.IndexOf(matches, match);
            matches[matchIndex] = matchWithStage;
        }
        return matches;
    }

    private async Task ClearDataAsync()
    {
        _matchRepository.DeleteAll();
        _fighterRepository.DeleteAll();
        _matchStageRepository.DeleteAll();
        _ratingRepository.DeleteAll();

        await _matchRepository.SaveChangesAsync();
        await _fighterRepository.SaveChangesAsync();
        await _matchStageRepository.SaveChangesAsync();
        await _ratingRepository.SaveChangesAsync();
    }

    
}
