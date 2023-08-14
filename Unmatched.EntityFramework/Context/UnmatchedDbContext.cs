namespace Unmatched.EntityFramework.Context;

using Microsoft.EntityFrameworkCore;

using Unmatched.Entities;

public class UnmatchedDbContext : DbContext
{
    public UnmatchedDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public DbSet<Fighter> Fighters { get; set; }

    public DbSet<Hero> Heroes { get; set; }

    public DbSet<Map> Maps { get; set; }

    public DbSet<Match> Matches { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<Rating> Ratings { get; set; }

    public DbSet<Sidekick> Sidekicks { get; set; }

    public DbSet<Tournament> Tournaments { get; set; }
}
