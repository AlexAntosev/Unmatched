namespace Unmatched.CatalogService.EntityFramework.Repositories;

using System;

public interface IUnitOfWork : IDisposable
{
    
    IHeroRepository Heroes { get; }
    
    IMapRepository Maps { get; }
    
    
    IMinionRepository Minions { get; }
    
    
    ISidekickRepository Sidekicks { get; }
    
    IVillainRepository Villains { get; }
    
    IPlayStyleRepository PlayStyles { get; }
    
    Task SaveChangesAsync();
}
