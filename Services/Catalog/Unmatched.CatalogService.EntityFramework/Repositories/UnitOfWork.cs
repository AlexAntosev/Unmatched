namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.EntityFramework.Context;

public class UnitOfWork(UnmatchedDbContext context) : IUnitOfWork
{
    public IHeroRepository Heroes { get; } = new HeroRepository(context);

    public IMapRepository Maps { get; } = new MapRepository(context);

    public IMinionRepository Minions { get; } = new MinionRepository(context);

    public ISidekickRepository Sidekicks { get; } = new SidekickRepository(context);

    public IVillainRepository Villains { get; } = new VillainRepository(context);

    public IPlayStyleRepository PlayStyles { get; } = new PlayStyleRepository(context);

    public Task SaveChangesAsync() 
        => context.SaveChangesAsync();

    public void Dispose() 
        => context.Dispose();
}
