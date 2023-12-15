namespace Unmatched.DataInitialization;

using Microsoft.EntityFrameworkCore;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services;
using Unmatched.Services.RatingCalculators;

public class RankedMatchDataInitializer : BaseMatchDataInitializer, IRankedMatchDataInitializer
{
    private readonly IMatchRepository _matchRepository;

    private readonly IRatingCalculator _ratingCalculator;

    private readonly IRatingRepository _ratingRepository;

    public RankedMatchDataInitializer(
        IHeroRepository heroRepository,
        IMapRepository mapRepository,
        IPlayerRepository playerRepository,
        IRatingRepository ratingRepository,
        IRatingCalculator ratingCalculator,
        IMatchRepository matchRepository,
        ITournamentRepository tournamentRepository)
        : base(heroRepository, mapRepository, playerRepository, tournamentRepository)
    {
        _ratingRepository = ratingRepository;
        _ratingCalculator = ratingCalculator;
        _matchRepository = matchRepository;
    }

    public IEnumerable<Match> GetEntities()
    {
        var matches = new List<Match>();
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("04/05/2023"),
                    MapId = GetMap(MapNames.Mansion),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Beowulf),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 2,
                                    HpLeft = 2,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 4
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.KingArthur),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 1,
                                    HpLeft = 0,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 0
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("04/13/2023"),
                    MapId = GetMap(MapNames.TRexPaddock),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.DrSattler),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 2,
                                    HpLeft = 8,
                                    SidekickHpLeft = 1,
                                    CardsLeft = 10
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Daredevil),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 1,
                                    HpLeft = 0,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 5
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("04/14/2023"),
                    MapId = GetMap(MapNames.RaptorPaddock),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.RobinHood),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 2,
                                    HpLeft = 8,
                                    SidekickHpLeft = 1,
                                    CardsLeft = 13
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Ingen),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 1,
                                    HpLeft = 0,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 16
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("04/17/2023"),
                    MapId = GetMap(MapNames.Ruins),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.TRex),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 1,
                                    HpLeft = 17,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 16
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.SherlokHolmes),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 2,
                                    HpLeft = 0,
                                    SidekickHpLeft = 7,
                                    CardsLeft = 18
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("04/17/2023"),
                    MapId = GetMap(MapNames.London),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.InvisibleMan),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 2,
                                    HpLeft = 6,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 7
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Sindbad),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 1,
                                    HpLeft = 0,
                                    SidekickHpLeft = 5,
                                    CardsLeft = 9
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("04/28/2023"),
                    MapId = GetMap(MapNames.Castle),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Raptors),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 1,
                                    HpLeft = 8,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 11
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Dracula),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 2,
                                    HpLeft = 0,
                                    SidekickHpLeft = 1,
                                    CardsLeft = 10
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("05/09/2023"),
                    MapId = GetMap(MapNames.Ship),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Medusa),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 1,
                                    HpLeft = 5,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 2
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.BloodyMary),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 2,
                                    HpLeft = 0,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 1
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("06/21/2023"),
                    MapId = GetMap(MapNames.HellsKitchen),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Ingen),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 2,
                                    HpLeft = 5,
                                    SidekickHpLeft = 1,
                                    CardsLeft = 7
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.SherlokHolmes),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 1,
                                    HpLeft = 0,
                                    SidekickHpLeft = 7,
                                    CardsLeft = 8
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("07/16/2023"),
                    MapId = GetMap(MapNames.TRexPaddock),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Alice),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 1,
                                    HpLeft = 1,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 11
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.Bullseye),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Andrii),
                                    Turn = 2,
                                    HpLeft = 0,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 16
                                },
                        }
                });
        
        matches.Add(
            new Match()
                {
                    Date = DateTime.Parse("08/11/2023"),
                    MapId = GetMap(MapNames.Tavern),
                    TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
                    Fighters = new List<Fighter>()
                        {
                            new()
                                {
                                    HeroId = GetHero(HeroNames.SherlokHolmes),
                                    IsWinner = true,
                                    PlayerId = GetPlayer(PlayerNames.Oleksandr),
                                    Turn = 2,
                                    HpLeft = 11,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 6
                                },
                            new()
                                {
                                    HeroId = GetHero(HeroNames.LittleRed),
                                    IsWinner = false,
                                    PlayerId = GetPlayer(PlayerNames.Ksuha),
                                    Turn = 1,
                                    HpLeft = 0,
                                    SidekickHpLeft = 0,
                                    CardsLeft = 0
                                },
                        }
                });
        
        return matches;
    }

    public async Task InitializeAsync()
    {
        var ratings = await _ratingRepository.Query().ToListAsync();
        var matches = GetEntities();
        foreach (var match in matches)
        {
            var fighterA = match.Fighters.First();
            var fighterB = match.Fighters.Last();

            var heroPoints = await _ratingCalculator.CalculateAsync(fighterA, fighterB);

            var fighterAPoints = heroPoints.First(x => x.HeroId == fighterA.HeroId).Points;
            var fighterBPoints = heroPoints.First(x => x.HeroId == fighterB.HeroId).Points;

            fighterA.MatchPoints = fighterAPoints;
            fighterB.MatchPoints = fighterBPoints;

            var fighterARating = ratings.FirstOrDefault(x => x.HeroId == fighterA.HeroId);
            if (fighterARating != null)
            {
                fighterARating.Points += fighterAPoints;
            }
            else
            {
                ratings.Add(
                    new Rating()
                        {
                            HeroId = fighterA.HeroId,
                            Points = fighterAPoints
                        });
            }

            var fighterBRating = ratings.FirstOrDefault(x => x.HeroId == fighterB.HeroId);
            if (fighterBRating != null)
            {
                fighterBRating.Points += fighterBPoints;
            }
            else
            {
                ratings.Add(
                    new Rating()
                        {
                            HeroId = fighterB.HeroId,
                            Points = fighterBPoints
                        });
            }
        }

        foreach (var rating in ratings)
        {
            _ratingRepository.AddOrUpdate(rating);
        }

        await _matchRepository.AddRangeAsync(matches);
        await _matchRepository.SaveChangesAsync();
        await _ratingRepository.SaveChangesAsync();
    }
}
