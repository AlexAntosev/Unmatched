namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class TournamentAwardRepository(UnmatchedDbContext dbContext)
    : BaseRepository<TournamentAwardEntity, UnmatchedDbContext>(dbContext), ITournamentAwardRepository
{
    public async Task<IReadOnlyList<TournamentAwardEntity>> GetByTournamentAsync(Guid tournamentId)
        => await DbContext.TournamentAwards.AsNoTracking().Where(a => a.TournamentId == tournamentId).ToListAsync();

    public async Task DeleteByAwardKindAsync(TournamentAwardKind awardKind)
    {
        var rows = await DbContext.TournamentAwards.Where(a => a.AwardKind == awardKind).ToListAsync();
        DbContext.TournamentAwards.RemoveRange(rows);
    }

    protected override Guid GetId(TournamentAwardEntity model)
        => model.Id;
}
