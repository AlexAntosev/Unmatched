namespace Unmatched.MatchService.EntityFramework.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using Unmatched.MatchService.Domain.Entities;

public class UnmatchedDbContextFactory : IDesignTimeDbContextFactory<UnmatchedDbContext>
{
    private readonly IConfiguration _configuration;

    public UnmatchedDbContextFactory()
    {
    }

    public UnmatchedDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public UnmatchedDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UnmatchedDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Unmatched_ms;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

        return new UnmatchedDbContext(optionsBuilder.Options);
    }
}

public class UnmatchedDbContext : DbContext
{
    public UnmatchedDbContext()
    {
    }
    
    public UnmatchedDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
    {
    }

    public DbSet<FighterEntity> Fighters { get; set; }

    public DbSet<HeroTitleEntity> HeroTitles { get; set; }


    public DbSet<MatchEntity> Matches { get; set; }


    public DbSet<RatingEntity> Ratings { get; set; }


    public DbSet<TournamentEntity> Tournaments { get; set; }

    public DbSet<TitleEntity> Titles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HeroTitleEntity>(b =>
            {
                b.HasKey(ht => new { ht.HeroesId, ht.TitlesId });
                b.ToTable("HeroTitle");
            });

        base.OnModelCreating(modelBuilder);
    }
}
