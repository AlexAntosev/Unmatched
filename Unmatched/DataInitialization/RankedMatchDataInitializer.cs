namespace Unmatched.DataInitialization;

using Microsoft.EntityFrameworkCore;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services;

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
        
        // matches.Add(
        //     new Match()
        //         {
        //             Date = DateTime.Parse("04/05/2023"),
        //             MapId = GetMap(MapNames.GoldenForest),
        //             TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Raptors),
        //                             IsWinner = true,
        //                             PlayerId = GetPlayer(PlayerNames.Andrii),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Bigfoot),
        //                             IsWinner = false,
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                 }
        //         });
        //
        // matches.Add(
        //     new Match()
        //         {
        //             Date = DateTime.Parse("04/05/2023"),
        //             MapId = GetMap(MapNames.GoldenForest),
        //             TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Raptors),
        //                             IsWinner = true,
        //                             PlayerId = GetPlayer(PlayerNames.Andrii),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Bigfoot),
        //                             IsWinner = false,
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                 }
        //         });
        //
        // matches.Add(
        //     new Match()
        //         {
        //             Date = DateTime.Parse("04/05/2023"),
        //             MapId = GetMap(MapNames.GoldenForest),
        //             TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Raptors),
        //                             IsWinner = true,
        //                             PlayerId = GetPlayer(PlayerNames.Andrii),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Bigfoot),
        //                             IsWinner = false,
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                 }
        //         });
        //
        // matches.Add(
        //     new Match()
        //         {
        //             Date = DateTime.Parse("04/05/2023"),
        //             MapId = GetMap(MapNames.GoldenForest),
        //             TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Raptors),
        //                             IsWinner = true,
        //                             PlayerId = GetPlayer(PlayerNames.Andrii),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Bigfoot),
        //                             IsWinner = false,
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                 }
        //         });
        //
        // matches.Add(
        //     new Match()
        //         {
        //             Date = DateTime.Parse("04/05/2023"),
        //             MapId = GetMap(MapNames.GoldenForest),
        //             TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Raptors),
        //                             IsWinner = true,
        //                             PlayerId = GetPlayer(PlayerNames.Andrii),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Bigfoot),
        //                             IsWinner = false,
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                 }
        //         });
        //
        // matches.Add(
        //     new Match()
        //         {
        //             Date = DateTime.Parse("04/05/2023"),
        //             MapId = GetMap(MapNames.GoldenForest),
        //             TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Raptors),
        //                             IsWinner = true,
        //                             PlayerId = GetPlayer(PlayerNames.Andrii),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Bigfoot),
        //                             IsWinner = false,
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                 }
        //         });
        //
        // matches.Add(
        //     new Match()
        //         {
        //             Date = DateTime.Parse("04/05/2023"),
        //             MapId = GetMap(MapNames.GoldenForest),
        //             TournamentId = GetTournament(TournamentNames.GoldenHalatLeague),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Raptors),
        //                             IsWinner = true,
        //                             PlayerId = GetPlayer(PlayerNames.Andrii),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Bigfoot),
        //                             IsWinner = false,
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr),
        //                             Turn = 1,
        //                             HpLeft = 0,
        //                             SidekickHpLeft = 0,
        //                             CardsLeft = 
        //                         },
        //                 }
        //         });
        //
        // matches.Add(
        //     new Match()
        //         {
        //             Date = DateTime.Parse("07/24/2023"),
        //             MapId = GetMap(MapNames.GoldenForest),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Raptors),
        //                             IsWinner = true,
        //                             PlayerId = GetPlayer(PlayerNames.Andrii)
        //                         },
        //                     new()
        //                         {
        //                             HeroId = GetHero(HeroNames.Bigfoot),
        //                             IsWinner = false,
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr)
        //                         },
        //                 }
        //         });
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
