namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class UnitOfWork : IUnitOfWork
{
    private readonly UnmatchedDbContext _context;

    public IFavoritesRepository Favorites { get; }

    public IFighterRepository Fighters { get; }

    public IMapRepository Maps { get; }

    public IMatchRepository Matches { get; }
    
    public IMinionRepository Minions { get; }

    public IPlayerRepository Players { get; }

    public IRatingRepository Ratings { get; }

    public ITournamentRepository Tournaments { get; }
    
    public ITitleRepository Titles { get; }
    
    public IVillainRepository Villains { get; }
    
    public UnitOfWork(UnmatchedDbContext context)
    {
        _context = context;
        Favorites = new FavoritesRepository(context);
        Fighters = new FighterRepository(context);
        Matches = new MatchRepository(context);
        Players = new PlayerRepository(context);
        Ratings = new RatingRepository(context);
        Tournaments = new TournamentRepository(context);
        Titles = new TitleRepository(context);
    }

    public Task SaveChangesAsync() 
        => _context.SaveChangesAsync();

    public void Dispose() 
        => _context.Dispose();
}
