namespace Unmatched.PlayerService.Tests;

using Unmatched.PlayerService.Domain.Entities;
using Unmatched.PlayerService.Domain.Repositories;
using Unmatched.PlayerService.Domain.Services;

/// <summary>
/// In-memory fakes rather than a mocking library - the repositories are simple enough that a
/// <see cref="List{T}"/>-backed fake reads clearer than a pile of mock setups.
/// </summary>
file class FakeFavoritesRepository : IFavoritesRepository
{
    public List<Favorite> Items { get; } = new();

    public Task<Favorite> AddAsync(Favorite model)
    {
        Items.Add(model);
        return Task.FromResult(model);
    }

    public void AddOrUpdate(Favorite model, Guid id)
    {
        var index = Items.FindIndex(f => f.Id == id);
        if (index < 0)
        {
            Items.Add(model);
        }
        else
        {
            Items[index] = model;
        }
    }

    public void AddOrUpdate(Favorite model) => AddOrUpdate(model, model.Id);

    public Task AddRangeAsync(IEnumerable<Favorite> models)
    {
        Items.AddRange(models);
        return Task.CompletedTask;
    }

    public Task Delete(Guid id)
    {
        Items.RemoveAll(f => f.Id == id);
        return Task.CompletedTask;
    }

    public void DeleteAll() => Items.Clear();

    public Task<List<Favorite>> GetAsync() => Task.FromResult(Items.ToList());

    public Task<Favorite?> GetByIdAsync(Guid id) => Task.FromResult(Items.FirstOrDefault(f => f.Id == id));

    public IQueryable<Favorite> Query(bool noTrack = false) => Items.AsQueryable();

    public Task SaveChangesAsync() => Task.CompletedTask;

    public Task<List<Favorite>> GetByPlayerIdAsync(Guid playerId)
        => Task.FromResult(Items.Where(f => f.PlayerId == playerId).ToList());
}

file class FakeUnitOfWork : IUnitOfWork
{
    public FakeFavoritesRepository FavoritesFake { get; } = new();

    public IFavoritesRepository Favorites => FavoritesFake;

    public IPlayerRepository Players => throw new NotSupportedException("Not needed by these tests.");

    public Task SaveChangesAsync() => Task.CompletedTask;

    public void Dispose()
    {
    }
}

public class FavoriteServiceTests
{
    private readonly Guid _playerId = Guid.NewGuid();
    private readonly Guid _heroId = Guid.NewGuid();

    [Fact]
    public async Task UpdateChosenOneAsync_HeroNeverFavoritedBefore_CreatesTheFavoriteRow()
    {
        var unitOfWork = new FakeUnitOfWork();
        var service = new FavoriteService(unitOfWork);

        var result = await service.UpdateChosenOneAsync(_playerId, _heroId, isChosenOne: true);

        Assert.Equal(_heroId, result);
        var favorite = Assert.Single(unitOfWork.FavoritesFake.Items);
        Assert.True(favorite.IsChosenOne);
        Assert.Equal(_playerId, favorite.PlayerId);
        Assert.Equal(_heroId, favorite.HeroId);
    }

    [Fact]
    public async Task UpdateChosenOneAsync_AnotherHeroWasMain_ClearsTheOldOneAndSetsTheNewOne()
    {
        var unitOfWork = new FakeUnitOfWork();
        var previousMainHeroId = Guid.NewGuid();
        unitOfWork.FavoritesFake.Items.Add(new Favorite { Id = Guid.NewGuid(), PlayerId = _playerId, HeroId = previousMainHeroId, IsChosenOne = true });
        var service = new FavoriteService(unitOfWork);

        await service.UpdateChosenOneAsync(_playerId, _heroId, isChosenOne: true);

        var previous = unitOfWork.FavoritesFake.Items.Single(f => f.HeroId == previousMainHeroId);
        var current = unitOfWork.FavoritesFake.Items.Single(f => f.HeroId == _heroId);
        Assert.False(previous.IsChosenOne);
        Assert.True(current.IsChosenOne);
    }

    [Fact]
    public async Task UpdateFavourAsync_HeroNeverFavoritedBefore_CreatesTheFavoriteRow()
    {
        var unitOfWork = new FakeUnitOfWork();
        var service = new FavoriteService(unitOfWork);

        await service.UpdateFavourAsync(_playerId, _heroId, favour: 4);

        var favorite = Assert.Single(unitOfWork.FavoritesFake.Items);
        Assert.Equal(4, favorite.Favour);
        Assert.False(favorite.IsChosenOne);
    }

    [Fact]
    public async Task UpdateFavourAsync_ExistingFavorite_UpdatesInPlaceWithoutDuplicating()
    {
        var unitOfWork = new FakeUnitOfWork();
        unitOfWork.FavoritesFake.Items.Add(new Favorite { Id = Guid.NewGuid(), PlayerId = _playerId, HeroId = _heroId, Favour = 2 });
        var service = new FavoriteService(unitOfWork);

        await service.UpdateFavourAsync(_playerId, _heroId, favour: 5);

        var favorite = Assert.Single(unitOfWork.FavoritesFake.Items);
        Assert.Equal(5, favorite.Favour);
    }
}
