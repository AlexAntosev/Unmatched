namespace Unmatched.StatisticsService.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IHeroStatsRepository HeroStats { get; }

    IMapStatsRepository MapStats { get; }

    IVillainStatsRepository VillainStats { get; }

    IMinionStatsRepository MinionStats { get; }

    Task SaveChangesAsync();
}
