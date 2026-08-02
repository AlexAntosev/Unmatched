namespace Unmatched.MatchService.EntityFramework.Repositories;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class TournamentParticipantRepository(UnmatchedDbContext dbContext)
    : BaseRepository<TournamentParticipantEntity, UnmatchedDbContext>(dbContext), ITournamentParticipantRepository
{
    protected override Guid GetId(TournamentParticipantEntity model)
        => model.Id;
}
