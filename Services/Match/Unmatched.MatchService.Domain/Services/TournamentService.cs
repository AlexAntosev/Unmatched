namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;
using Unmatched.MatchService.Domain.Repositories;

public class TournamentService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICatalogHeroCache catalogHeroCache,
    ICatalogMapCache catalogMapCache) : ITournamentService
{
    public async Task<Tournament> AddAsync(Tournament dto)
    {
        var tournament = mapper.Map<TournamentEntity>(dto);
        var created = await unitOfWork.Tournaments.AddAsync(tournament);
        await unitOfWork.SaveChangesAsync();
        var createdDto = mapper.Map<Tournament>(created);
        return createdDto;
    }

    public async Task<IEnumerable<Tournament>> GetAsync()
    {
        var entities = await unitOfWork.Tournaments.GetAsync();
        var tournaments = mapper.Map<IEnumerable<Tournament>>(entities);

        return tournaments;
    }

    public async Task<Tournament> GetAsync(Guid id)
    {
        var entity = await unitOfWork.Tournaments.GetByIdAsync(id);
        var tournament = mapper.Map<Tournament>(entity);

        return tournament;
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Tournaments.Delete(id);
        await unitOfWork.SaveChangesAsync();
    }
    
    public async Task CreateInitialPlannedMatchesAsync(Tournament tournament)
    {
        var heroesEntities = await catalogHeroCache.GetAsync();
        var heroes = mapper.Map<List<FighterHero>>(heroesEntities);
        heroes = heroes.Shuffle().Take(tournament.CurrentStage.GetFightersCount()).ToList();
        await CreatePlannedMatchesAsync(tournament, heroes);
    }
    
    public async Task CreateNextStagePlannedMatchesAsync(Tournament tournament)
    {
        var currentStageMatches = await unitOfWork.Matches.GetByTournamentAsync(tournament.Id);
        var currentStageWinners = currentStageMatches.
            Where(m => m.Stage == tournament.CurrentStage && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner))
            .Select(f => mapper.Map<FighterHero>(catalogHeroCache.GetAsync(f.HeroId)))
            .ToList();
        
        tournament.CurrentStage++;
        
        await CreatePlannedMatchesAsync(tournament, currentStageWinners);
    }
    
    public async Task CreateThirdPlaceDeciderMatchAsync(Tournament tournament)
    {
        var currentStageMatches = await unitOfWork.Matches.GetByTournamentAsync(tournament.Id);
        var currentStageLosers = currentStageMatches.
            Where(m => m.Stage == tournament.CurrentStage && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => !f.IsWinner))
            .Select(f => mapper.Map<FighterHero>(catalogHeroCache.GetAsync(f.HeroId)))
            .ToList();
        
        tournament.CurrentStage++;
        
        await CreatePlannedMatchesAsync(tournament, currentStageLosers);
    }
    
    public async Task CreateGrandFinalMatchesAsync(Tournament tournament)
    {
        var currentStageMatches = await unitOfWork.Matches.GetByTournamentAsync(tournament.Id);
        var currentStageWinners = currentStageMatches.
            Where(m => m.Stage == tournament.CurrentStage - 1 && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner))
            .Select(f => mapper.Map<FighterHero>(catalogHeroCache.GetAsync(f.HeroId)))
            .ToList();
        
        tournament.CurrentStage++;

        var maps = (await catalogMapCache.GetAsync()).ToList();
        for (var j = 0; j < 2; j++)
        {
            var finalists = currentStageWinners.Clone();

            var participants = new List<Fighter>();
            foreach (var hero in finalists)
            {
                var participant = new Fighter
                    {
                        Hero = hero,
                        HeroId = hero.Id
                    };
                participants.Add(participant);
            }

            var generatedMatches = new List<Match>();
            for (var i = 0; i < participants.Count; i += 2)
            {
                var players = new List<Guid>()
                    {
                        AndriiAndOlexPlayerIds.Andrii,
                        AndriiAndOlexPlayerIds.Olex
                    };
                var turns = new List<int>
                    {
                        1,
                        2
                    };
            
                var fighter = participants[i];
                var opponent = participants[i + 1];

                if (j == 0)
                {
                    fighter.PlayerId = players[0];
                    fighter.Turn = turns[0];
                    opponent.PlayerId = players[1];
                    opponent.Turn = turns[1];
                }
                else
                {
                    fighter.PlayerId = players[1];
                    fighter.Turn = turns[1];
                    opponent.PlayerId = players[0];
                    opponent.Turn = turns[0];
                }
              
            
                var match = new Match
                    {
                        Id = Guid.Empty,
                        Stage = tournament.CurrentStage,
                        Fighters = new List<Fighter>
                            {
                                fighter,
                                opponent
                            },
                        TournamentId = tournament.Id,
                        MapId = maps.GetAndRemoveRandomItem().Id,
                        IsPlanned = true
                    };
                generatedMatches.Add(match);
            }

            await UpdateAsync(tournament.Id, generatedMatches, tournament.CurrentStage);
        }
        
        
        await CreatePlannedMatchesAsync(tournament, currentStageWinners);
    }
    
    private async Task UpdateAsync(Guid id, IEnumerable<Match> matches, Stage stage)
    {
        var matchEntities = mapper.Map<IEnumerable<MatchEntity>>(matches);

        foreach (var matchEntity in matchEntities)
        {
            await unitOfWork.Matches.AddAsync(matchEntity);
        }

        var tournament = await unitOfWork.Tournaments.GetByIdAsync(id);
        
        tournament.CurrentStage = stage;
        unitOfWork.Tournaments.AddOrUpdate(tournament);
        
        await unitOfWork.SaveChangesAsync();
    }
    
    private async Task CreatePlannedMatchesAsync(Tournament tournament, List<FighterHero> heroes)
    {
        var maps = (await catalogMapCache.GetAsync()).ToList();

        heroes = heroes.Shuffle();
        var participants = new List<Fighter>();
        foreach (var hero in heroes)
        {
            var participant = new Fighter
                {
                    Hero = hero,
                    HeroId = hero.Id
                };
            participants.Add(participant);
        }

        var generatedMatches = new List<Match>();
        for (var i = 0; i < participants.Count; i += 2)
        {
            var players = new List<Guid>()
                {
                    AndriiAndOlexPlayerIds.Andrii,
                    AndriiAndOlexPlayerIds.Olex
                };
            var turns = new List<int>
                {
                    1,
                    2
                };
            
            var fighter = participants[i];
            fighter.PlayerId = players.GetAndRemoveRandomItem();
            fighter.Turn = turns.GetAndRemoveRandomItem();
            var opponent = participants[i + 1];
            opponent.PlayerId = players.GetAndRemoveRandomItem();
            opponent.Turn = turns.GetAndRemoveRandomItem();
            
            var match = new Match
                {
                    Id = Guid.Empty,
                    Stage = tournament.CurrentStage,
                    Fighters = new List<Fighter>
                        {
                            fighter,
                            opponent
                        },
                    TournamentId = tournament.Id,
                    MapId = maps.GetAndRemoveRandomItem().Id,
                    IsPlanned = true
                };
            generatedMatches.Add(match);
        }

        await UpdateAsync(tournament.Id, generatedMatches, tournament.CurrentStage);
    }
}
