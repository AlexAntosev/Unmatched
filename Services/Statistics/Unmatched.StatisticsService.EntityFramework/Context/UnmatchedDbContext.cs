namespace Unmatched.StatisticsService.EntityFramework.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using Unmatched.StatisticsService.EntityFramework.Entities;

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
        optionsBuilder.UseSqlServer(
            "Server=localhost\\SQLEXPRESS;Database=ms.Unmatched.Statistics;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

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

    public DbSet<HeroStatsEntity> HeroStats { get; set; }

    public DbSet<MapStatsEntity> MapStats { get; set; }
}
