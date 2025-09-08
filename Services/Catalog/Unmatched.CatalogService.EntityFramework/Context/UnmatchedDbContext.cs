namespace Unmatched.CatalogService.EntityFramework.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using Unmatched.CatalogService.Domain.Entities;

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

    public DbSet<Hero> Heroes { get; set; }

    public DbSet<Map> Maps { get; set; }
    
    public DbSet<Minion> Minions { get; set; }

    public DbSet<Sidekick> Sidekicks { get; set; }
    
    
    public DbSet<Villain> Villains { get; set; }
    
    public DbSet<PlayStyle> PlayStyles { get; set; }
}
