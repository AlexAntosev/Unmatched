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
        optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Unmatched;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

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

    public DbSet<Expansion> Expansions { get; set; }

    public DbSet<OwnedExpansion> OwnedExpansions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expansion>()
            .HasIndex(e => e.Name)
            .IsUnique();

        modelBuilder.Entity<OwnedExpansion>()
            .HasIndex(o => o.ExpansionId)
            .IsUnique();

        modelBuilder.Entity<OwnedExpansion>()
            .HasOne(o => o.Expansion)
            .WithMany()
            .HasForeignKey(o => o.ExpansionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Hero>()
            .HasOne(h => h.Expansion)
            .WithMany(e => e.Heroes)
            .HasForeignKey(h => h.ExpansionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Map>()
            .HasOne(m => m.Expansion)
            .WithMany(e => e.Maps)
            .HasForeignKey(m => m.ExpansionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Villain>()
            .HasOne(v => v.Expansion)
            .WithMany(e => e.Villains)
            .HasForeignKey(v => v.ExpansionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Minion>()
            .HasOne(m => m.Expansion)
            .WithMany(e => e.Minions)
            .HasForeignKey(m => m.ExpansionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
