namespace Unmatched.Services;

using System.Text.Json;
using AutoMapper;

using Unmatched.Data.Entities;
using Unmatched.Data.Enums;
using Unmatched.Data.Repositories;
using Unmatched.Dtos;
using Unmatched.Extensions;

public class TournamentService : ITournamentService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public TournamentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<TournamentDto> AddAsync(TournamentDto dto)
    {
        var tournament = _mapper.Map<Tournament>(dto);
        var created = await _unitOfWork.Tournaments.AddAsync(tournament);
        await _unitOfWork.SaveChangesAsync();
        var createdDto = _mapper.Map<TournamentDto>(created);
        return createdDto;
    }

    public async Task<IEnumerable<TournamentDto>> GetAsync()
    {
        var entities = await _unitOfWork.Tournaments.GetAsync();
        var tournaments = _mapper.Map<IEnumerable<TournamentDto>>(entities);

        return tournaments;
    }

    public async Task<TournamentDto> GetAsync(Guid id)
    {
        var entity = await _unitOfWork.Tournaments.GetByIdAsync(id);
        var tournament = _mapper.Map<TournamentDto>(entity);

        return tournament;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _unitOfWork.Tournaments.Delete(id);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task CreateInitialPlannedMatchesAsync(TournamentDto tournament)
    {
        var heroesEntities = await _unitOfWork.Heroes.GetAsync();
        var heroes = _mapper.Map<List<HeroDto>>(heroesEntities);
        heroes = heroes.Shuffle().Take(tournament.CurrentStage.GetFightersCount()).ToList();
        await CreatePlannedMatchesAsync(tournament, heroes);
    }
    
    public async Task CreateNextStagePlannedMatchesAsync(TournamentDto tournament)
    {
        var currentStageWinners = tournament.Matches.
            Where(m => m.Stage == tournament.CurrentStage && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner))
            .Select(f => f.Hero)
            .ToList();
        
        tournament.CurrentStage++;
        
        await CreatePlannedMatchesAsync(tournament, currentStageWinners);
    }
    
    public async Task CreateThirdPlaceDeciderMatchAsync(TournamentDto tournament)
    {
        var currentStageLosers = tournament.Matches.
            Where(m => m.Stage == tournament.CurrentStage && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => !f.IsWinner))
            .Select(f => f.Hero)
            .ToList();
        
        tournament.CurrentStage++;
        
        await CreatePlannedMatchesAsync(tournament, currentStageLosers);
    }
    
    public async Task CreateGrandFinalMatchesAsync(TournamentDto tournament)
    {
        var currentStageWinners = tournament.Matches.
            Where(m => m.Stage == tournament.CurrentStage - 1 && !m.IsPlanned)
            .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner))
            .Select(f => f.Hero)
            .ToList();
        
        tournament.CurrentStage++;

        var maps = await _unitOfWork.Maps.GetAsync();
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
                var players = await _unitOfWork.Players.GetOleksAndAndrewAsync();
                var turns = new List<int>
                    {
                        1,
                        2
                    };
            
                var fighter = participants[i];
                var opponent = participants[i + 1];

                if (j == 0)
                {
                    fighter.PlayerId = players[0].Id;
                    fighter.Turn = turns[0];
                    opponent.PlayerId = players[1].Id;
                    opponent.Turn = turns[1];
                }
                else
                {
                    fighter.PlayerId = players[1].Id;
                    fighter.Turn = turns[1];
                    opponent.PlayerId = players[0].Id;
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
        var matchEntities = _mapper.Map<IEnumerable<Match>>(matches);

        foreach (var matchEntity in matchEntities)
        {
            await _unitOfWork.Matches.AddAsync(matchEntity);
        }

        var tournament = await _unitOfWork.Tournaments.GetByIdAsync(id);
        
        tournament.CurrentStage = stage;
        _unitOfWork.Tournaments.AddOrUpdate(tournament);
        
        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task CreatePlannedMatchesAsync(TournamentDto tournament, List<HeroDto> heroes)
    {
        var maps = await _unitOfWork.Maps.GetAsync();

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
            var players = await _unitOfWork.Players.GetOleksAndAndrewAsync();
            var turns = new List<int>
                {
                    1,
                    2
                };
            
            var fighter = participants[i];
            fighter.PlayerId = players.GetAndRemoveRandomItem().Id;
            fighter.Turn = turns.GetAndRemoveRandomItem();
            var opponent = participants[i + 1];
            opponent.PlayerId = players.GetAndRemoveRandomItem().Id;
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
