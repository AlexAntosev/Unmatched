namespace Unmatched.DataInitialization;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;

public class HoudiniVsGenieDadaInitializer : IHoudiniVsGenieDadaInitializer
{
    private readonly IMapRepository _mapRepository;

    private readonly ISidekickRepository _sidekickRepository;

    private readonly IHeroRepository _heroRepository;

    public HoudiniVsGenieDadaInitializer(IMapRepository mapRepository, ISidekickRepository sidekickRepository, IHeroRepository heroRepository)
    {
        _mapRepository = mapRepository;
        _sidekickRepository = sidekickRepository;
        _heroRepository = heroRepository;
    }

   

    public async Task InitializeAsync()
    {
        try
        {
            _mapRepository.GetIdByName(MapNames.KingSolomonsMine);
            return;
        }
        catch (Exception e)
        {

        }
        
        var bess = await _sidekickRepository.AddAsync(new Sidekick()
            {
                Name = SidekickNames.Bess,
                Hp = 5,
                Count = 1,
                IsRanged = false
            });
        await _sidekickRepository.SaveChangesAsync();
        
        var heroes = new List<Hero>()
            {
                new()
                    {
                        Name = HeroNames.TheGenie,
                        Hp = 16,
                        DeckSize = 30,
                        IsRanged = true
                    },
                new()
                    {
                        Name = HeroNames.Houdini,
                        Hp = 14,
                        DeckSize = 30,
                        IsRanged = false,
                        Sidekicks = new List<Sidekick>
                            {
                                bess
                            }
                    }
            };

        await _heroRepository.AddRangeAsync(heroes);
        await _heroRepository.SaveChangesAsync();
        
        var map = new Map()
            {
                Name = MapNames.KingSolomonsMine
            };

        await _mapRepository.AddAsync(map);
        await _mapRepository.SaveChangesAsync();
    }
}
