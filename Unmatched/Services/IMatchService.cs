namespace Unmatched.Services;

using Unmatched.Dtos;
using Unmatched.Entities;

public interface IMatchService
{
    public Task AddAsync(Match match);
    
    public Task AddAsync(MatchDto match, FighterDto fighter, FighterDto opponent);
}
