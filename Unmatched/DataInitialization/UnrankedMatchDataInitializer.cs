namespace Unmatched.DataInitialization;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

public class UnrankedMatchDataInitializer : BaseMatchDataInitializer, IUnrankedMatchDataInitializer
{
    private readonly IMatchRepository _matchRepository;

    public UnrankedMatchDataInitializer(
        IHeroRepository heroRepository,
        IPlayerRepository playerRepository,
        IMatchRepository matchRepository,
        IMapRepository mapRepository,
        ITournamentRepository tournamentRepository)
        : base(heroRepository, mapRepository, playerRepository, tournamentRepository)
    {
        _matchRepository = matchRepository;
    }

    public IEnumerable<Match> GetEntities()
    {
        var matches = new List<Match>();
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/20/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Dracula),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Alice),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Tetyana)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/24/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Sindbad),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.InvisibleMan),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Tetyana)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/25/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Alice),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Bigfoot),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/25/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.RobinHood),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.KingArthur),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/31/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.JakylAndHide),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.InvisibleMan),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/31/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.KingArthur),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.SherlokHolmes),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("02/05/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.SherlokHolmes),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.RobinHood),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("02/19/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.InvisibleMan),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Medusa),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("02/20/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Alice),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Sindbad),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("05/31/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Bigfoot),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Dracula),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("06/05/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Medusa),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.SherlokHolmes),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("06/12/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.KingArthur),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Medusa),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Tetyana)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("07/17/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Elektra),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.MoonKnight),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("08/25/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.InvisibleMan),
                                    IsWinner = false,
                                    HpLeft = 0,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Bullseye),
                                    IsWinner = true,
                                    HpLeft = 1,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("09/30/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.LittleRed),
                                    IsWinner = true,
                                    HpLeft = 7,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Ghostrider),
                                    IsWinner = false,
                                    HpLeft = 0,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("10/31/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Daredevil),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Beowulf),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("11/03/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Dracula),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.InvisibleMan),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("11/03/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.MoonKnight),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.LittleRed),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("11/05/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.SunWukong),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Achilles),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("11/06/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Sindbad),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Medusa),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Ksuha)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("11/06/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.KingArthur),
                                    IsWinner = false,
                                    HpLeft = 0,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Alice),
                                    IsWinner = true,
                                    HpLeft = 1,
                                    PlayerId = GetPlayer(PlayerNames.Ksuha)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("11/08/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Bullseye),
                                    IsWinner = true,
                                    HpLeft = 7,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.PrincessYennenga),
                                    IsWinner = false,
                                    HpLeft = 0,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("11/08/2022"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Ghostrider),
                                    IsWinner = false,
                                    HpLeft = 0,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.BloodyMary),
                                    IsWinner = true,
                                    HpLeft = 1,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/16/2023"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Bigfoot),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.JakylAndHide),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/17/2023"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.MoonKnight),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.KingArthur),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("01/18/2023"),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Beowulf),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Elektra),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("07/22/2023"),
                    MapId = GetMap(MapNames.Mansion),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.LittleRed),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.MoonKnight),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("07/24/2023"),
                    MapId = GetMap(MapNames.GreenForest),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.JakylAndHide),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.TRex),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("07/24/2023"),
                    MapId = GetMap(MapNames.GoldenForest),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Raptors),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Bigfoot),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("07/24/2023"),
                    MapId = GetMap(MapNames.Castle),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Elektra),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Dracula),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("08/01/2023"),
                    MapId = GetMap(MapNames.Ruins),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.SunWukong),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii)
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.LukeCage),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr)
                                },
                        }
                });

        return matches;
    }

    public async Task InitializeAsync()
    {
        var entities = GetEntities();
        await _matchRepository.AddRangeAsync(entities);
        await _matchRepository.SaveChangesAsync();
    }
}
