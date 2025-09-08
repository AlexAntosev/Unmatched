namespace Unmatched.MatchService.EntityFramework.Repositories;

using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class UnitOfWork(UnmatchedDbContext context) : IUnitOfWork
{
    public IFighterRepository Fighters { get; } = new FighterRepository(context);

    public IMatchRepository Matches { get; } = new MatchRepository(context);

    public IRatingRepository Ratings { get; } = new RatingRepository(context);

    public ITournamentRepository Tournaments { get; } = new TournamentRepository(context);

    public ITitleRepository Titles { get; } = new TitleRepository(context);

    public Task SaveChangesAsync()
        => context.SaveChangesAsync();

    public void Dispose()
        => context.Dispose();
}
