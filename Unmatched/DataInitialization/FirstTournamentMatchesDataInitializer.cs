namespace Unmatched.DataInitialization;

using Microsoft.EntityFrameworkCore;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.MatchHandlers;
using Unmatched.Services.RatingCalculators;

class FirstTournamentMatchesDataInitializer : BaseMatchDataInitializer, IFirstTournamentMatchesDataInitializer
{
    private readonly IFirstTournamentRatingCalculator _ratingCalculator;
    private readonly IMatchStageRepository _matchStageRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IFighterRepository _fighterRepository;
    private readonly IMatchRepository _matchRepository;

    public FirstTournamentMatchesDataInitializer(
        IHeroRepository heroRepository,
        IMapRepository mapRepository,
        IRatingRepository ratingRepository,
        IFighterRepository fighterRepository,
        IMatchRepository matchRepository,
        ITournamentRepository tournamentRepository,
        IPlayerRepository playerRepository,
        IFirstTournamentRatingCalculator ratingCalculator,
        IMatchStageRepository matchStageRepository) : base(heroRepository, mapRepository, playerRepository, tournamentRepository)
    {
        _ratingRepository = ratingRepository;
        _matchRepository = matchRepository;
        _fighterRepository = fighterRepository;
        _ratingCalculator = ratingCalculator;
        _matchStageRepository = matchStageRepository;
    }

    public IEnumerable<Match> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task InitializeAsync()
    {
        var groupStageMatches = new List<MatchWithStage>()
            {
                new() {Date = new DateTime(2022, 11, 11),MapId = GetMap(MapNames.GreenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.InvisibleMan), HpLeft = 7, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.RobinHood), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 11),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.PrincessYennenga), HpLeft = 7, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.JakylAndHide), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 11),MapId = GetMap(MapNames.Laboratory), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Bigfoot), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 11),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Ghostrider), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 13),MapId = GetMap(MapNames.Ruins), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.KingArthur), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Medusa), HpLeft = 15, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 13),MapId = GetMap(MapNames.HellsKitchen), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.MoonKnight), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Elektra), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 13),MapId = GetMap(MapNames.London), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Bullseye), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Sindbad), HpLeft = 5, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 14),MapId = GetMap(MapNames.Mansion), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Dracula), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 14),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.InvisibleMan), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Beowulf), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 14),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.InvisibleMan), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Beowulf), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 17),MapId = GetMap(MapNames.Tavern), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LittleRed), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.MoonKnight), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 17),MapId = GetMap(MapNames.Tavern), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Daredevil), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.BloodyMary), HpLeft = 10, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 17),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Alice), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.SherlokHolmes), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 18),MapId = GetMap(MapNames.Ruins), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.RobinHood), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.KingArthur), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 18),MapId = GetMap(MapNames.Mansion), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.PrincessYennenga), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Elektra), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 21),MapId = GetMap(MapNames.Laboratory), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Bullseye), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 21),MapId = GetMap(MapNames.Castle), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 23),MapId = GetMap(MapNames.GreenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Medusa), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Beowulf), HpLeft = 5, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 23),MapId = GetMap(MapNames.Mansion), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.JakylAndHide), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.LittleRed), HpLeft = 7, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 23),MapId = GetMap(MapNames.Ship), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Daredevil), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Bigfoot), HpLeft = 8, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 24),MapId = GetMap(MapNames.London), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Alice), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Dracula), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 24),MapId = GetMap(MapNames.Tavern), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Medusa), HpLeft = 14, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.InvisibleMan), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 25),MapId = GetMap(MapNames.HellsKitchen), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LittleRed), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Elektra), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 26),MapId = GetMap(MapNames.Ruins), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.BloodyMary), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Sindbad), HpLeft = 5, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 29),MapId = GetMap(MapNames.Castle), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.SherlokHolmes), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Ghostrider), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 29),MapId = GetMap(MapNames.Ship), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Beowulf), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.KingArthur), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 30),MapId = GetMap(MapNames.Ruins), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.PrincessYennenga), HpLeft = 4, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.MoonKnight), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 11, 30),MapId = GetMap(MapNames.Ship), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Sindbad), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 01),MapId = GetMap(MapNames.Mansion), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 11, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Alice), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 03),MapId = GetMap(MapNames.London), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.RobinHood), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Beowulf), HpLeft = 7, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 03),MapId = GetMap(MapNames.Castle), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.JakylAndHide), HpLeft = 4, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.MoonKnight), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 04),MapId = GetMap(MapNames.Laboratory), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Sindbad), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Bigfoot), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 04),MapId = GetMap(MapNames.Ship), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.SherlokHolmes), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Dracula), HpLeft = 7, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 06),MapId = GetMap(MapNames.HellsKitchen), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.RobinHood), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Medusa), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 11),MapId = GetMap(MapNames.Castle), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LittleRed), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.PrincessYennenga), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 11),MapId = GetMap(MapNames.Ship), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Daredevil), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Bullseye), HpLeft = 8, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 12),MapId = GetMap(MapNames.Ruins), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 5, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Alice), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 12),MapId = GetMap(MapNames.HellsKitchen), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.InvisibleMan), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.KingArthur), HpLeft = 9, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 14),MapId = GetMap(MapNames.GreenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Elektra), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.JakylAndHide), HpLeft = 4, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 14),MapId = GetMap(MapNames.London), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.BloodyMary), HpLeft = 9, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 14),MapId = GetMap(MapNames.Ruins), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Alice), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Ghostrider), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 15),MapId = GetMap(MapNames.GreenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.BloodyMary), HpLeft = 5, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Bullseye), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 15),MapId = GetMap(MapNames.London), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.SherlokHolmes), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 18),MapId = GetMap(MapNames.HellsKitchen), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Daredevil), HpLeft = 12, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Sindbad), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 20),MapId = GetMap(MapNames.Castle), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Dracula), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Ghostrider), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 20),MapId = GetMap(MapNames.Ruins), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 11, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Daredevil), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 20),MapId = GetMap(MapNames.GreenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 10, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.SherlokHolmes), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 21),MapId = GetMap(MapNames.London), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Bigfoot), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Bullseye), HpLeft = 4, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 21),MapId = GetMap(MapNames.Castle), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 6, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Dracula), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 22),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Bigfoot), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.BloodyMary), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 22),MapId = GetMap(MapNames.Mansion), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Ghostrider), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}
            };
        
        var quarterFinalsMatches = new List<MatchWithStage>()
            {
                new() {Date = new DateTime(2022, 12, 24),MapId = GetMap(MapNames.GreenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Beowulf), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 15, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 24),MapId = GetMap(MapNames.Castle), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LittleRed), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 25),MapId = GetMap(MapNames.HellsKitchen), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.PrincessYennenga), HpLeft = 4, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.BloodyMary), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 25),MapId = GetMap(MapNames.Tavern), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.RobinHood), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 2, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}
            };

        var semiFinalsMatches = new List<MatchWithStage>()
            {
                new() {Date = new DateTime(2022, 12, 27),MapId = GetMap(MapNames.GoldenForest), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2022, 12, 29),MapId = GetMap(MapNames.Laboratory), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.PrincessYennenga), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}
            };
        
        var thirdPlaceFinalsMatches = new List<MatchWithStage>()
            {
                new() {Date = new DateTime(2022, 12, 29),MapId = GetMap(MapNames.Mansion), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.LukeCage), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.PrincessYennenga), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}
            };

        var finalsMatches = new List<MatchWithStage>()
            {
                new() {Date = new DateTime(2023, 1, 2),MapId = GetMap(MapNames.Ship), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 3, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }},
                new() {Date = new DateTime(2023, 1, 2),MapId = GetMap(MapNames.London), TournamentId = GetTournament(TournamentNames.UnmatchedFirstTournament), Fighters = new List<Fighter>()
                    {
                        new() {HeroId = GetHero(HeroNames.SunWukong), HpLeft = 1, IsWinner = true, PlayerId = GetPlayer(PlayerNames.Andrii)},
                        new() {HeroId = GetHero(HeroNames.Achilles), HpLeft = 0, IsWinner = false, PlayerId = GetPlayer(PlayerNames.Oleksandr)}
                    }}
            };
        
        var handler = new FirstTournamentMatchHandler(_ratingCalculator, _matchRepository, _ratingRepository, _fighterRepository, _matchStageRepository);

        foreach (var match in groupStageMatches)
        {
            await InsertMatchStage(match, Stage.Group);
        }
        
        foreach (var match in quarterFinalsMatches)
        {
            await InsertMatchStage(match, Stage.QuarterFinals);
        }

        foreach (var match in semiFinalsMatches)
        {
            await InsertMatchStage(match, Stage.SemiFinals);
        }
        
        foreach (var match in thirdPlaceFinalsMatches)
        {
            await InsertMatchStage(match, Stage.ThirdPlaceFinals);
        }
        
        foreach (var match in finalsMatches)
        {
            await InsertMatchStage(match, Stage.Finals);
        }

        await _matchStageRepository.SaveChangesAsync();
    }

    private async Task InsertMatchStage(MatchWithStage match, Stage stage)
    {
        var existingMatch = FindExistingMatch(match);
        var matchStage = new MatchStage()
            {
                MatchId = existingMatch.Id,
                Stage = stage
            };
        await _matchStageRepository.AddAsync(matchStage);
    }

    private Match FindExistingMatch(MatchWithStage match)
    {
        var existingMatch = _matchRepository.Query()
            .Include(x => x.Fighters)
            .Where(
                x => x.TournamentId.Equals(match.TournamentId)
                 && x.Date.Equals(match.Date)
                 && x.MapId.Equals(match.MapId)).ToArray().SingleOrDefault(x => x.Fighters.First(f => f.IsWinner).HeroId.Equals(match.Fighters.First(f => f.IsWinner).HeroId));
        if (existingMatch == null)
        {
            throw new InvalidOperationException();
        }

        return existingMatch;
    }
}
