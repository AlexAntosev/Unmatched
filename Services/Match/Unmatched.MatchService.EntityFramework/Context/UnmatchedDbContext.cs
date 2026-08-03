namespace Unmatched.MatchService.EntityFramework.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

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

    public DbSet<FighterEntity> Fighters { get; set; }

    public DbSet<HeroTitleEntity> HeroTitles { get; set; }


    public DbSet<MatchEntity> Matches { get; set; }

    public DbSet<MatchVillainEntity> MatchVillains { get; set; }

    public DbSet<MatchMinionEntity> MatchMinions { get; set; }


    public DbSet<RatingEntity> Ratings { get; set; }

    public DbSet<RatingRecalculationStateEntity> RatingRecalculationStates { get; set; }

    public DbSet<TournamentEntity> Tournaments { get; set; }

    public DbSet<TournamentParticipantEntity> TournamentParticipants { get; set; }

    public DbSet<TournamentTitleEntity> TournamentTitles { get; set; }

    public DbSet<TournamentAwardEntity> TournamentAwards { get; set; }

    public DbSet<TitleEntity> Titles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HeroTitleEntity>(b =>
            {
                b.HasKey(ht => new { ht.HeroesId, ht.TitlesId });
                b.ToTable("HeroTitle");
            });

        modelBuilder.Entity<MatchEntity>()
            .HasOne(m => m.Villain)
            .WithOne()
            .HasForeignKey<MatchVillainEntity>(v => v.MatchId);

        modelBuilder.Entity<RatingEntity>()
            .HasIndex(r => r.HeroId)
            .IsUnique();

        modelBuilder.Entity<TournamentEntity>()
            .HasMany(t => t.Participants)
            .WithOne()
            .HasForeignKey(p => p.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TournamentParticipantEntity>()
            .HasIndex(p => new { p.TournamentId, p.HeroId })
            .IsUnique();

        modelBuilder.Entity<TournamentEntity>()
            .HasMany(t => t.TournamentTitles)
            .WithOne()
            .HasForeignKey(t => t.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TournamentTitleEntity>()
            .HasIndex(t => new { t.TournamentId, t.Kind })
            .IsUnique();

        // BountyChallengeWin is excluded: a hero can defend a Bounty pool many times, producing many
        // award rows with the same (TournamentId, HeroId, AwardKind) - every other kind is a one-time
        // completion bonus, so it stays unique.
        modelBuilder.Entity<TournamentAwardEntity>()
            .HasIndex(a => new { a.TournamentId, a.HeroId, a.AwardKind })
            .IsUnique()
            .HasFilter($"([AwardKind] <> {(int)TournamentAwardKind.BountyChallengeWin})");

        modelBuilder.Entity<TitleEntity>()
            .HasIndex(t => t.RuleKey)
            .IsUnique()
            .HasFilter("[RuleKey] IS NOT NULL");

        base.OnModelCreating(modelBuilder);
    }
}
