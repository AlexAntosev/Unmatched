namespace Unmatched.EntityFramework.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Unmatched.Entities;

public class UnmatchedDbContext : DbContext
{
    public UnmatchedDbContext()
    {
    }
    
    public UnmatchedDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
    }
    
    public class UnmatchedDbContextFactory : IDesignTimeDbContextFactory<UnmatchedDbContext>
    {
        public UnmatchedDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UnmatchedDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Unmatched;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new UnmatchedDbContext(optionsBuilder.Options);
        }
    }

    public DbSet<Fighter> Fighters { get; set; }

    public DbSet<Hero> Heroes { get; set; }

    public DbSet<Map> Maps { get; set; }

    public DbSet<Match> Matches { get; set; }
    
    public DbSet<MatchStage> MatchStages { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<Rating> Ratings { get; set; }

    public DbSet<Sidekick> Sidekicks { get; set; }

    public DbSet<Tournament> Tournaments { get; set; }
}
