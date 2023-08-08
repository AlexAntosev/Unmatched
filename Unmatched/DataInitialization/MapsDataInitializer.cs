using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

namespace Unmatched.DataInitialization;

public class MapsDataInitializer : IDataInitializer<Map>
{
    private readonly IMapRepository _mapRepository;

    public MapsDataInitializer(IMapRepository mapRepository)
    {
        _mapRepository = mapRepository;
    }

    public async Task InitializeAsync()
    {
        var defaultMaps = new List<Map>
        {
            new() { Name = MapNames.Castle },
            new() { Name = MapNames.Ship },
            new() { Name = MapNames.GreenForest },
            new() { Name = MapNames.GoldenForest },
            new() { Name = MapNames.London },
            new() { Name = MapNames.Mansion },
            new() { Name = MapNames.Tavern },
            new() { Name = MapNames.Laboratory },
            new() { Name = MapNames.HellsKitchen },
            new() { Name = MapNames.Ruins },
            new() { Name = MapNames.RaptorPaddock },
            new() { Name = MapNames.TRexPaddock }
        };
        
        await _mapRepository.AddRangeAsync(defaultMaps);
        await _mapRepository.SaveChangesAsync();
    }
}