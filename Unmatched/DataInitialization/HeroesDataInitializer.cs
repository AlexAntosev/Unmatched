namespace Unmatched.DataInitialization;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

public class HeroesDataInitializer : IDataInitializer<Hero>
{
    private readonly IHeroRepository _heroRepository;

    private readonly ISidekickRepository _sidekickRepository;

    public HeroesDataInitializer(IHeroRepository heroRepository, ISidekickRepository sidekickRepository)
    {
        _heroRepository = heroRepository;
        _sidekickRepository = sidekickRepository;
    }

    public IEnumerable<Hero> GetEntities()
    {
        var defaultSidekicks = _sidekickRepository.Query().ToList();

        var merlin = defaultSidekicks.First(s => s.Name == SidekickNames.Merlin);
        var harpies = defaultSidekicks.First(s => s.Name == SidekickNames.Harpies);
        var porter = defaultSidekicks.First(s => s.Name == SidekickNames.ThePorter);
        var jabberwock = defaultSidekicks.First(s => s.Name == SidekickNames.TheJabberwock);
        var outlaws = defaultSidekicks.First(s => s.Name == SidekickNames.Outlaws);
        var jackalope = defaultSidekicks.First(s => s.Name == SidekickNames.TheJackalope);
        var sisters = defaultSidekicks.First(s => s.Name == SidekickNames.TheSisters);
        var watson = defaultSidekicks.First(s => s.Name == SidekickNames.DrWatson);
        var huntsman = defaultSidekicks.First(s => s.Name == SidekickNames.Huntsman);
        var wiglaf = defaultSidekicks.First(s => s.Name == SidekickNames.Wiglaf);
        var hand = defaultSidekicks.First(s => s.Name == SidekickNames.TheHand);
        var mistyKnight = defaultSidekicks.First(s => s.Name == SidekickNames.MistyKnight);
        var archers = defaultSidekicks.First(s => s.Name == SidekickNames.Archers);
        var patroclus = defaultSidekicks.First(s => s.Name == SidekickNames.Patroclus);
        var malcolm = defaultSidekicks.First(s => s.Name == SidekickNames.DrMalcolm);

        var workers = defaultSidekicks.First(s => s.Name == SidekickNames.IngenWorkers);
        return new List<Hero>
            {
                new()
                    {
                        Name = HeroNames.KingArthur,
                        Hp = 18,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                merlin
                            }
                    },
                new()
                    {
                        Name = HeroNames.Medusa,
                        Hp = 16,
                        DeckSize = 30,
                        IsRanged = true,
                        Sidekicks = new List<Sidekick>
                            {
                                harpies
                            }
                    },
                new()
                    {
                        Name = HeroNames.Sindbad,
                        Hp = 15,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                porter
                            }
                    },
                new()
                    {
                        Name = HeroNames.Alice,
                        Hp = 13,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                jabberwock
                            }
                    },
                new()
                    {
                        Name = HeroNames.RobinHood,
                        Hp = 13,
                        DeckSize = 30,
                        IsRanged = true,
                        Sidekicks = new List<Sidekick>
                            {
                                outlaws
                            }
                    },
                new()
                    {
                        Name = HeroNames.Bigfoot,
                        Hp = 16,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                jackalope
                            }
                    },
                new()
                    {
                        Name = HeroNames.Dracula,
                        Hp = 13,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                sisters
                            }
                    },
                new()
                    {
                        Name = HeroNames.JakylAndHide,
                        Hp = 16,
                        DeckSize = 30
                    },
                new()
                    {
                        Name = HeroNames.InvisibleMan,
                        Hp = 15,
                        DeckSize = 30
                    },
                new()
                    {
                        Name = HeroNames.SherlokHolmes,
                        Hp = 16,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                watson
                            }
                    },
                new()
                    {
                        Name = HeroNames.LittleRed,
                        Hp = 14,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                huntsman
                            }
                    },
                new()
                    {
                        Name = HeroNames.Beowulf,
                        Hp = 17,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                wiglaf
                            }
                    },
                new()
                    {
                        Name = HeroNames.Elektra,
                        Hp = 16,
                        DeckSize = 40,
                        Sidekicks = new List<Sidekick>
                            {
                                hand
                            }
                    },
                new()
                    {
                        Name = HeroNames.Daredevil,
                        Hp = 17,
                        DeckSize = 22
                    },
                new()
                    {
                        Name = HeroNames.Bullseye,
                        Hp = 14,
                        DeckSize = 30,
                        IsRanged = true
                    },
                new()
                    {
                        Name = HeroNames.Ghostrider,
                        Hp = 17,
                        DeckSize = 30
                    },
                new()
                    {
                        Name = HeroNames.MoonKnight,
                        Hp = 16,
                        DeckSize = 30
                    },
                new()
                    {
                        Name = HeroNames.LukeCage,
                        Hp = 13,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                mistyKnight
                            }
                    },
                new()
                    {
                        Name = HeroNames.BloodyMary,
                        Hp = 16,
                        DeckSize = 30
                    },
                new()
                    {
                        Name = HeroNames.PrincessYennega,
                        Hp = 15,
                        DeckSize = 30,
                        IsRanged = true,
                        Sidekicks = new List<Sidekick>
                            {
                                archers
                            }
                    },
                new()
                    {
                        Name = HeroNames.Achilles,
                        Hp = 18,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                patroclus
                            }
                    },
                new()
                    {
                        Name = HeroNames.SunWukong,
                        Hp = 17,
                        DeckSize = 30
                    },
                new()
                    {
                        Name = HeroNames.DrSattler,
                        Hp = 13,
                        DeckSize = 30,
                        Sidekicks = new List<Sidekick>
                            {
                                malcolm
                            }
                    },
                new()
                    {
                        Name = HeroNames.TRex,
                        Hp = 27,
                        DeckSize = 30
                    },
                new()
                    {
                        Name = HeroNames.Raptors,
                        Hp = 21,
                        DeckSize = 30
                    },
                new()
                    {
                        Name = HeroNames.Ingen,
                        Hp = 14,
                        DeckSize = 30,
                        IsRanged = true,
                        Sidekicks = new List<Sidekick>
                            {
                                workers
                            }
                    }
            };
    }

    public async Task InitializeAsync()
    {
        var defaultHeroes = GetEntities();

        await _heroRepository.AddRangeAsync(defaultHeroes);
        await _heroRepository.SaveChangesAsync();
    }
}
