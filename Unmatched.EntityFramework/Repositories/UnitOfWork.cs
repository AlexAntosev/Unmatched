namespace Unmatched.EntityFramework.Repositories;

using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly UnmatchedDbContext _context;

    public IFighterRepository Fighters { get; }

    public IHeroRepository Heroes { get; }

    public IMapRepository Maps { get; }

    public IMatchRepository Matches { get; }

    public IMatchStageRepository MatchStages { get; }

    public IPlayerRepository Players { get; }

    public IRatingRepository Ratings { get; }

    public ISidekickRepository Sidekicks { get; }

    public ITournamentRepository Tournaments { get; }
    
    public ITitleRepository Titles { get; }
    
    public UnitOfWork(UnmatchedDbContext context)
    {
        _context = context;
        Fighters = new FighterRepository(context);
        Heroes = new HeroRepository(context);
        Maps = new MapRepository(context);
        Matches = new MatchRepository(context);
        MatchStages = new MatchStageRepository(context);
        Players = new PlayerRepository(context);
        Ratings = new RatingRepository(context);
        Sidekicks = new SidekickRepository(context);
        Tournaments = new TournamentRepository(context);
        Titles = new TitleRepository(context);
    }

    public Task SaveChangesAsync() 
        => _context.SaveChangesAsync();

    public void Dispose() 
        => _context.Dispose();
}
