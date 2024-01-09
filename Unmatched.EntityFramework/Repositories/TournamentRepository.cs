namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public TournamentRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tournament> AddAsync(Tournament model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Tournament model)
    {
        _dbContext.Update(model);
    }

    public async Task AddRangeAsync(IEnumerable<Tournament> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Tournaments.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Tournaments;
        _dbContext.Tournaments.RemoveRange(entities);
    }

    public async Task<Tournament> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Tournaments.Include(t => t.Matches).ThenInclude(m => m.Fighters).ThenInclude(f => f.Hero).FirstOrDefaultAsync(t => t.Id == id);

        return entity;
    }

    public IQueryable<Tournament> Query()
    {
        return _dbContext.Tournaments;
    }

    public async Task<List<Tournament>> GetAsync()
    {
        return await _dbContext.Tournaments.OrderBy(h => h.Name).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Guid GetIdByName(string name)
    {
        return _dbContext.Tournaments.First(x => x.Name.Equals(name)).Id;
    }
}
