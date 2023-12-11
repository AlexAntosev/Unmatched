namespace Unmatched.DataInitialization;

using Microsoft.EntityFrameworkCore;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services;

class FirstTournamentMatchesDataInitializer : BaseMatchDataInitializer, IFirstTournamentMatchesDataInitializer
{
    private readonly IFirstTournamentRatingCalculator _ratingCalculator;

    private readonly IRatingRepository _ratingRepository;

    private readonly IMatchRepository _matchRepository;

    public FirstTournamentMatchesDataInitializer(
        IHeroRepository heroRepository,
        IMapRepository mapRepository,
        IRatingRepository ratingRepository,
        IMatchRepository matchRepository,
        ITournamentRepository tournamentRepository,
        IPlayerRepository playerRepository,
        IFirstTournamentRatingCalculator ratingCalculator) : base(heroRepository, mapRepository, playerRepository, tournamentRepository)
    {
        _ratingRepository = ratingRepository;
        _matchRepository = matchRepository;
        _ratingCalculator = ratingCalculator;
    }

    public IEnumerable<Match> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task InitializeAsync()
    {
        var matches = new List<FirstTournamentMatchInfo>()
                {
                    new("11/17/2022", GetMap(MapNames.Tavern), GetHero(HeroNames.LittleRed), 2, GetHero(HeroNames.MoonKnight), 0, MatchLevel.Group),
                    new("11/17/2022", GetMap(MapNames.Tavern), GetHero(HeroNames.Daredevil), 0, GetHero(HeroNames.BloodyMary), 10, MatchLevel.Group),
                    new("11/17/2022", GetMap(MapNames.GoldenForest), GetHero(HeroNames.Alice), 3, GetHero(HeroNames.SherlokHolmes), 0, MatchLevel.Group),

                    new("11/18/2022", GetMap(MapNames.Ruins), GetHero(HeroNames.RobinHood), 2, GetHero(HeroNames.KingArthur), 0, MatchLevel.Group),
                    new("11/18/2022", GetMap(MapNames.Mansion), GetHero(HeroNames.PrincessYennenga), 0, GetHero(HeroNames.Elektra), 6, MatchLevel.Group),
                    new("11/21/2022", GetMap(MapNames.Laboratory), GetHero(HeroNames.LukeCage), 3, GetHero(HeroNames.Bullseye), 0, MatchLevel.Group),
                    new("11/21/2022", GetMap(MapNames.Castle), GetHero(HeroNames.Achilles), 0, GetHero(HeroNames.SunWukong), 6, MatchLevel.Group),
                    new("11/23/2022", GetMap(MapNames.GreenForest), GetHero(HeroNames.Medusa), 0, GetHero(HeroNames.Beowulf), 5, MatchLevel.Group),
                    new("11/23/2022", GetMap(MapNames.Mansion), GetHero(HeroNames.JakylAndHide), 0, GetHero(HeroNames.LittleRed), 7, MatchLevel.Group),
                    new("11/23/2022", GetMap(MapNames.Ship), GetHero(HeroNames.Daredevil), 0, GetHero(HeroNames.Bigfoot), 8, MatchLevel.Group),
                    new("11/24/2022", GetMap(MapNames.London), GetHero(HeroNames.Alice), 1, GetHero(HeroNames.Dracula), 0, MatchLevel.Group),
                    new("11/24/2022", GetMap(MapNames.Tavern), GetHero(HeroNames.Medusa), 14, GetHero(HeroNames.InvisibleMan), 0, MatchLevel.Group),
                    new("11/25/2022", GetMap(MapNames.HellsKitchen), GetHero(HeroNames.LittleRed), 1, GetHero(HeroNames.Elektra), 0, MatchLevel.Group),
                    new("11/26/2022", GetMap(MapNames.Ruins), GetHero(HeroNames.BloodyMary), 0, GetHero(HeroNames.Sindbad), 5, MatchLevel.Group),
                    new("11/29/2022", GetMap(MapNames.Castle), GetHero(HeroNames.SherlokHolmes), 0, GetHero(HeroNames.Ghostrider), 2, MatchLevel.Group),
                    new("11/29/2022", GetMap(MapNames.Ship), GetHero(HeroNames.Beowulf), 1, GetHero(HeroNames.KingArthur), 0, MatchLevel.Group),
                    new("11/30/2022", GetMap(MapNames.Ruins), GetHero(HeroNames.PrincessYennenga), 4, GetHero(HeroNames.MoonKnight), 0, MatchLevel.Group),
                    new("11/30/2022", GetMap(MapNames.Ship), GetHero(HeroNames.Sindbad), 0, GetHero(HeroNames.LukeCage), 2, MatchLevel.Group),

                    new("12/01/2022", GetMap(MapNames.Mansion), GetHero(HeroNames.Achilles), 11, GetHero(HeroNames.Alice), 0, MatchLevel.Group),
                    new("12/03/2022", GetMap(MapNames.London), GetHero(HeroNames.RobinHood), 0, GetHero(HeroNames.Beowulf), 7, MatchLevel.Group),
                    new("12/03/2022", GetMap(MapNames.Castle), GetHero(HeroNames.JakylAndHide), 4, GetHero(HeroNames.MoonKnight), 0, MatchLevel.Group),
                    new("12/04/2022", GetMap(MapNames.Laboratory), GetHero(HeroNames.Sindbad), 3, GetHero(HeroNames.Bigfoot), 0, MatchLevel.Group),
                    new("12/04/2022", GetMap(MapNames.Ship), GetHero(HeroNames.SherlokHolmes), 0, GetHero(HeroNames.Dracula), 7, MatchLevel.Group),
                    new("12/06/2022", GetMap(MapNames.HellsKitchen), GetHero(HeroNames.RobinHood), 1, GetHero(HeroNames.Medusa), 0, MatchLevel.Group),
                    new("12/11/2022", GetMap(MapNames.Castle), GetHero(HeroNames.LittleRed), 2, GetHero(HeroNames.PrincessYennenga), 0, MatchLevel.Group),
                    new("12/11/2022", GetMap(MapNames.Ship), GetHero(HeroNames.Daredevil), 0, GetHero(HeroNames.Bullseye), 8, MatchLevel.Group),
                    new("12/12/2022", GetMap(MapNames.Ruins), GetHero(HeroNames.SunWukong), 5, GetHero(HeroNames.Alice), 0, MatchLevel.Group),
                    new("12/12/2022", GetMap(MapNames.HellsKitchen), GetHero(HeroNames.InvisibleMan), 0, GetHero(HeroNames.KingArthur), 9, MatchLevel.Group),
                    new("12/14/2022", GetMap(MapNames.GreenForest), GetHero(HeroNames.Elektra), 0, GetHero(HeroNames.JakylAndHide), 4, MatchLevel.Group),
                    new("12/14/2022", GetMap(MapNames.London), GetHero(HeroNames.BloodyMary), 9, GetHero(HeroNames.LukeCage), 0, MatchLevel.Group),
                    new("12/14/2022", GetMap(MapNames.Ruins), GetHero(HeroNames.Alice), 1, GetHero(HeroNames.Ghostrider), 0, MatchLevel.Group),
                    new("12/15/2022", GetMap(MapNames.GreenForest), GetHero(HeroNames.BloodyMary), 5, GetHero(HeroNames.Bullseye), 0, MatchLevel.Group),
                    new("12/15/2022", GetMap(MapNames.London), GetHero(HeroNames.SherlokHolmes), 0, GetHero(HeroNames.SunWukong), 1, MatchLevel.Group),
                    new("12/18/2022", GetMap(MapNames.HellsKitchen), GetHero(HeroNames.Daredevil), 12, GetHero(HeroNames.Sindbad), 0, MatchLevel.Group),
                    new("12/20/2022", GetMap(MapNames.Castle), GetHero(HeroNames.Dracula), 3, GetHero(HeroNames.Ghostrider), 0, MatchLevel.Group),
                    new("12/20/2022", GetMap(MapNames.Ruins), GetHero(HeroNames.LukeCage), 11, GetHero(HeroNames.Daredevil), 0, MatchLevel.Group),
                    new("12/20/2022", GetMap(MapNames.GreenForest), GetHero(HeroNames.Achilles), 10, GetHero(HeroNames.SherlokHolmes), 0, MatchLevel.Group),
                    new("12/21/2022", GetMap(MapNames.London), GetHero(HeroNames.Bigfoot), 0, GetHero(HeroNames.Bullseye), 4, MatchLevel.Group),
                    new("12/21/2022", GetMap(MapNames.Castle), GetHero(HeroNames.SunWukong), 6, GetHero(HeroNames.Dracula), 0, MatchLevel.Group),
                    new("12/22/2022", GetMap(MapNames.GoldenForest), GetHero(HeroNames.Bigfoot), 3, GetHero(HeroNames.BloodyMary), 0, MatchLevel.Group),
                    new("12/22/2022", GetMap(MapNames.Mansion), GetHero(HeroNames.Ghostrider), 0, GetHero(HeroNames.Achilles), 2, MatchLevel.Group),

                    new("12/24/2022", GetMap(MapNames.GreenForest), GetHero(HeroNames.Beowulf), 0, GetHero(HeroNames.Achilles), 15, MatchLevel.QuarterFinals),
                    new("12/24/2022", GetMap(MapNames.Castle), GetHero(HeroNames.LittleRed), 0, GetHero(HeroNames.LukeCage), 2, MatchLevel.QuarterFinals),
                    new("12/25/2022", GetMap(MapNames.HellsKitchen), GetHero(HeroNames.PrincessYennenga), 4, GetHero(HeroNames.BloodyMary), 0, MatchLevel.QuarterFinals),
                    new("12/25/2022", GetMap(MapNames.Tavern), GetHero(HeroNames.RobinHood), 0, GetHero(HeroNames.SunWukong), 2, MatchLevel.QuarterFinals),

                    new("12/27/2022", GetMap(MapNames.GoldenForest), GetHero(HeroNames.Achilles), 1, GetHero(HeroNames.LukeCage), 0, MatchLevel.SemiFinals),
                    new("12/29/2022", GetMap(MapNames.Laboratory), GetHero(HeroNames.PrincessYennenga), 0, GetHero(HeroNames.SunWukong), 3, MatchLevel.SemiFinals),

                    new("12/29/2022", GetMap(MapNames.Mansion), GetHero(HeroNames.LukeCage), 1, GetHero(HeroNames.PrincessYennenga), 0, MatchLevel.ThirdPlaceFinals),

                    new("01/02/2023", GetMap(MapNames.Ship), GetHero(HeroNames.Achilles), 0, GetHero(HeroNames.SunWukong), 3, MatchLevel.Finals),
                    new("01/02/2023", GetMap(MapNames.London), GetHero(HeroNames.SunWukong), 1, GetHero(HeroNames.Achilles), 0, MatchLevel.Finals),

                }.OrderBy(x => x.Date)
            .ToArray();
        
        var m = new List<Match>()
            {
                new() {Date = new DateTime(2022, 11, 11),MapId = GetMap(MapNames.GreenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.InvisibleMan), HpLeft = 7, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.RobinHood), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 11),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.PrincessYennenga), HpLeft = 7, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.JakylAndHide), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 11),MapId = GetMap(MapNames.Laboratory), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Bigfoot), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 11),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Ghostrider), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 13),MapId = GetMap(MapNames.Ruins), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.KingArthur), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Medusa), HpLeft = 15, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 13),MapId = GetMap(MapNames.HellsKitchen), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.MoonKnight), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Elektra), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 13),MapId = GetMap(MapNames.London), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Bullseye), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Sindbad), HpLeft = 5, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 14),MapId = GetMap(MapNames.Mansion), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Dracula), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 14),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.InvisibleMan), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Beowulf), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
                new() {Date = new DateTime(2022, 11, 14),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.InvisibleMan), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Beowulf), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}, //MatchLevel.Group
            };

        // we assume that this are the very first rating matches so we do not read existing ratings 
        var ratings = new Dictionary<Guid, int>();

        // foreach (var match in m)
        // {
        //     var heroPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last(), MatchLevel.Group);
        //     var andriiPoints = heroPoints.First(x => x.HeroId == match.AndriiHeroId).Points;
        //     var olexPoints = heroPoints.First(x => x.HeroId == match.OlexHeroId).Points;
        //
        //     var matchEntity = new Match()
        //         {
        //             Date = match.Date,
        //             MapId = match.MapId,
        //             TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament),
        //             Fighters = new List<Fighter>()
        //                 {
        //                     new()
        //                         {
        //                             PlayerId = GetPlayer(PlayerNames.Andrii),
        //                             IsWinner = match.AndriiHp > 0,
        //                             HpLeft = match.AndriiHp,
        //                             HeroId = match.AndriiHeroId,
        //                             MatchPoints = andriiPoints,
        //                         },
        //                     new()
        //                         {
        //                             PlayerId = GetPlayer(PlayerNames.Oleksandr),
        //                             IsWinner = match.OlexHp > 0,
        //                             HpLeft = match.OlexHp,
        //                             HeroId = match.OlexHeroId,
        //                             MatchPoints = olexPoints,
        //                         }
        //                 }
        //         };
        //
        //     await _matchRepository.AddAsync(matchEntity);
        //     
        //     if (ratings.TryGetValue(match.AndriiHeroId, out var apoints))
        //     {
        //         ratings[match.AndriiHeroId] = apoints + andriiPoints;
        //     }
        //     else
        //     {
        //         ratings[match.AndriiHeroId] = andriiPoints;
        //     }
        //
        //     if (ratings.TryGetValue(match.OlexHeroId, out var opoints))
        //     {
        //         ratings[match.OlexHeroId] = opoints + olexPoints;
        //     }
        //     else
        //     {
        //         ratings[match.OlexHeroId] = olexPoints;
        //     }
        // }
        //
        //
        // await _ratingRepository.AddRangeAsync(
        //     ratings.Select(
        //         x => new Rating()
        //             {
        //                 HeroId = x.Key,
        //                 Points = x.Value
        //             }));
        //
        //
        // await _matchRepository.SaveChangesAsync();
        // await _ratingRepository.SaveChangesAsync();
    }
}
