namespace Unmatched.DataInitialization;

using System;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;

public class TalesOfAmazeDataInitializer : ITalesOfAmazeDataInitializer
{
    private readonly IUnitOfWork _unitOfWork;

    public TalesOfAmazeDataInitializer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task InitializeAsync()
    {
        try
        {
            _unitOfWork.Maps.GetIdByName(MapNames.McMinnville);
            return;
        }
        catch (Exception e)
        {

        }
        
        var daisy = await _unitOfWork.Sidekicks.AddAsync(new Sidekick
            {
                Name = SidekickNames.Daisy,
                Hp = 6,
                Count = 1,
                IsRanged = false
            });
        var charlie = await _unitOfWork.Sidekicks.AddAsync(new Sidekick
            {
                Name = SidekickNames.Charlie,
                Hp = 8,
                Count = 1,
                IsRanged = true
            });
        await _unitOfWork.SaveChangesAsync();
        
        var heroes = new List<Hero>()
            {
                new()
                    {
                        Name = HeroNames.DrJillTrent,
                        Hp = 13,
                        DeckSize = 30,
                        IsRanged = false,
                        Sidekicks = new List<Sidekick>
                            {
                                daisy
                            },
                        Color = "#09587c"
                    },
                new()
                    {
                        Name = HeroNames.AnnieChristmas,
                        Hp = 14,
                        DeckSize = 30,
                        IsRanged = false,
                        Sidekicks = new List<Sidekick>
                            {
                                charlie
                            },
                        Color = "#4d0d8a"
                    },
                new()
                    {
                        Name = HeroNames.NikolaTesla,
                        Hp = 14,
                        DeckSize = 30,
                        IsRanged = true,
                        Color = "#d5710d"
                    },
                new()
                    {
                        Name = HeroNames.GoldenBat,
                        Hp = 18,
                        DeckSize = 30,
                        IsRanged = false,
                        Color = "#c19a0c"
                    }
            };

        await _unitOfWork.Heroes.AddRangeAsync(heroes);
        await _unitOfWork.SaveChangesAsync();
        
        var mcMinnville = new Map()
            {
                Name = MapNames.McMinnville
            }; 
        var pointPleasant = new Map()
            {
                Name = MapNames.PointPleasant
            };

        await _unitOfWork.Maps.AddAsync(mcMinnville);
        await _unitOfWork.Maps.AddAsync(pointPleasant);
        await _unitOfWork.SaveChangesAsync();
    }
}
