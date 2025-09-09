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
    public async Task<TournamentDto> AddAsync(TournamentDto dto)
    {
        var tournament = mapper.Map<TournamentEntity>(dto);
        var created = await unitOfWork.Tournaments.AddAsync(tournament);
        await unitOfWork.SaveChangesAsync();
        var createdDto = mapper.Map<TournamentDto>(created);
        return createdDto;
    }

    public async Task<IEnumerable<TournamentDto>> GetAsync()
    {
        var entities = await unitOfWork.Tournaments.GetAsync();
        var tournaments = mapper.Map<IEnumerable<TournamentDto>>(entities);

        return tournaments;
    }

    public async Task<TournamentDto> GetAsync(Guid id)
    {
        var entity = await unitOfWork.Tournaments.GetByIdAsync(id);
        var tournament = mapper.Map<TournamentDto>(entity);

        return tournament;
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Tournaments.Delete(id);
        await unitOfWork.SaveChangesAsync();
    }
    
    public async Task CreateInitialPlannedMatchesAsync(TournamentDto tournament)
    {
        var heroesEntities = await catalogHeroCache.GetAsync();
        var heroes = mapper.Map<List<FighterHeroDto>>(heroesEntities);
        heroes = heroes.Shuffle().Take(tournament.CurrentStage.GetFightersCount()).ToList();
        await CreatePlannedMatchesAsync(tournament, heroes);
    }
    
    public async Task CreateNextStagePlannedMatchesAsync(TournamentDto tournament)
    {
        var currentStageMatches = await unitOfWork.Matches.GetByTournamentAsync(tournament.Id);
        var currentStageWinners = currentStageMatches.
            Where(m => m.Stage == tournament.CurrentStage && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner))
            .Select(f => mapper.Map<FighterHeroDto>(catalogHeroCache.GetAsync(f.HeroId)))
            .ToList();
        
        tournament.CurrentStage++;
        
        await CreatePlannedMatchesAsync(tournament, currentStageWinners);
    }
    
    public async Task CreateThirdPlaceDeciderMatchAsync(TournamentDto tournament)
    {
        var currentStageMatches = await unitOfWork.Matches.GetByTournamentAsync(tournament.Id);
        var currentStageLosers = currentStageMatches.
            Where(m => m.Stage == tournament.CurrentStage && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => !f.IsWinner))
            .Select(f => mapper.Map<FighterHeroDto>(catalogHeroCache.GetAsync(f.HeroId)))
            .ToList();
        
        tournament.CurrentStage++;
        
        await CreatePlannedMatchesAsync(tournament, currentStageLosers);
    }
    
    public async Task CreateGrandFinalMatchesAsync(TournamentDto tournament)
    {
        var currentStageMatches = await unitOfWork.Matches.GetByTournamentAsync(tournament.Id);
        var currentStageWinners = currentStageMatches.
            Where(m => m.Stage == tournament.CurrentStage - 1 && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner))
            .Select(f => mapper.Map<FighterHeroDto>(catalogHeroCache.GetAsync(f.HeroId)))
            .ToList();
        
        tournament.CurrentStage++;

        var maps = (await catalogMapCache.GetAsync()).ToList();
        for (var j = 0; j < 2; j++)
        {
            var finalists = currentStageWinners.Clone();

            var participants = new List<FighterDto>();
            foreach (var hero in finalists)
            {
                var participant = new FighterDto
                    {
                        Hero = hero,
                        HeroId = hero.Id
                    };
                participants.Add(participant);
            }

            var generatedMatches = new List<MatchDto>();
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
              
            
                var match = new MatchDto
                    {
                        Id = Guid.Empty,
                        Stage = tournament.CurrentStage,
                        Fighters = new List<FighterDto>
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
    
    private async Task UpdateAsync(Guid id, IEnumerable<MatchDto> matches, Stage stage)
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
    
    private async Task CreatePlannedMatchesAsync(TournamentDto tournament, List<FighterHeroDto> heroes)
    {
        var maps = (await catalogMapCache.GetAsync()).ToList();

        heroes = heroes.Shuffle();
        var participants = new List<FighterDto>();
        foreach (var hero in heroes)
        {
            var participant = new FighterDto
                {
                    Hero = hero,
                    HeroId = hero.Id
                };
            participants.Add(participant);
        }

        var generatedMatches = new List<MatchDto>();
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
            
            var match = new MatchDto
                {
                    Id = Guid.Empty,
                    Stage = tournament.CurrentStage,
                    Fighters = new List<FighterDto>
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
