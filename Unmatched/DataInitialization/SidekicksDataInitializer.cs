using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

namespace Unmatched.DataInitialization;

public class SidekicksDataInitializer : IDataInitializer<Sidekick>
{
    private readonly ISidekickRepository _sidekickRepository;

    public SidekicksDataInitializer(ISidekickRepository sidekickRepository)
    {
        _sidekickRepository = sidekickRepository;
    }

    public async Task InitializeAsync()
    {
        var defaultSidekicks = GetEntities();
        
        await _sidekickRepository.AddRangeAsync(defaultSidekicks);
        await _sidekickRepository.SaveChangesAsync();
    }

    public IEnumerable<Sidekick> GetEntities()
    {
        return  new List<Sidekick>
        {
            new() { Name = SidekickNames.Merlin, Hp = 7, IsRanged = true },
            new() { Name = SidekickNames.Harpies, Hp = 1, Count = 3 },
            new() { Name = SidekickNames.ThePorter, Hp = 6 },
            new() { Name = SidekickNames.TheJabberwock, Hp = 8 },
            new() { Name = SidekickNames.Outlaws, Hp = 1, Count = 4 },
            new() { Name = SidekickNames.TheJackalope, Hp = 6 },
            new() { Name = SidekickNames.TheSisters, Hp = 1, Count = 3 },
            new() { Name = SidekickNames.DrWatson, Hp = 8, IsRanged = true },
            new() { Name = SidekickNames.Huntsman, Hp = 9, IsRanged = true },
            new() { Name = SidekickNames.Wiglaf, Hp = 9 },
            new() { Name = SidekickNames.TheHand, Hp = 1, Count = 4 },
            new() { Name = SidekickNames.MistyKnight, Hp = 6, IsRanged = true },
            new() { Name = SidekickNames.Archers, Hp = 2, Count = 2, IsRanged = true },
            new() { Name = SidekickNames.Patroclus, Hp = 6 },
            new() { Name = SidekickNames.DrMalcolm, Hp = 7 },
            new() { Name = SidekickNames.IngenWorkers, Hp = 1, Count = 3, IsRanged = true }
        };
    }
}