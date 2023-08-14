namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public PlayerRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Player> AddAsync(Player model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Player model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Player> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Players.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public async Task<Player> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Players.FindAsync(id);

        return entity;
    }

    public IQueryable<Player> Query()
    {
        return _dbContext.Players;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Guid GetIdByName(string name)
    {
        return _dbContext.Players.First(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Id;
    }
}
