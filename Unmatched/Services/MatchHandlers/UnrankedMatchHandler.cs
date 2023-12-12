namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;

public class UnrankedMatchHandler : BaseMatchHandler
{
    private readonly IMatchRepository _matchRepository;

    private readonly IFighterRepository _fighterRepository;

    public UnrankedMatchHandler(IMatchRepository matchRepository, IFighterRepository fighterRepository)
    {
        _matchRepository = matchRepository;
        _fighterRepository = fighterRepository;
    }
    protected override async Task InnerHandleAsync(Match match)
    {
        await _matchRepository.AddAsync(match);

        await _matchRepository.SaveChangesAsync();
        await _fighterRepository.SaveChangesAsync();
    }
}
