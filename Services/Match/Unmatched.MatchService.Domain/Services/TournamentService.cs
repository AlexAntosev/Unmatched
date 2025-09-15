namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Models.Catalog;
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

    public async Task CreateNextStagePlannedMatchesAsync(Guid tournamentId)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            throw new KeyNotFoundException($"No tournament with key {tournamentId}");
        }

        var currentStageMatches = await unitOfWork.Matches.GetByTournamentAsync(tournament.Id);

        if (currentStageMatches.Any(m => m.IsPlanned))
        {
            throw new InvalidOperationException("Cannot generate matches for the next stage. Current stage is still in progress. Please finish current stage first.");
        }

        IEnumerable<CatalogHeroDto> catalogHeroes;

        if (currentStageMatches.Any() == false)
        {
            var allCatalogHeroes = await catalogHeroCache.GetAsync();
            catalogHeroes = allCatalogHeroes.ToList().Shuffle().Take(tournament.CurrentStage.GetFightersCount()).ToList();
        }
        else
        {
            var getHeroTasks = tournament.CurrentStage switch
                {
                    Stage.SemiFinals => currentStageMatches.Where(m => m.Stage == tournament.CurrentStage)
                        .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner == false))
                        .Select(f => catalogHeroCache.GetAsync(f.HeroId))
                        .ToList(),
                    Stage.ThirdPlaceDecider => currentStageMatches.Where(m => m.Stage == tournament.CurrentStage - 1)
                        .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner))
                        .Select(f => catalogHeroCache.GetAsync(f.HeroId))
                        .ToList(),
                    _ => currentStageMatches.Where(m => m.Stage == tournament.CurrentStage)
                        .Select(m => m.Fighters.FirstOrDefault(f => f.IsWinner))
                        .Select(f => catalogHeroCache.GetAsync(f.HeroId))
                        .ToList()
                };

            catalogHeroes = (await Task.WhenAll(getHeroTasks))!;
            tournament.CurrentStage++;
        }

        var heroes = catalogHeroes.Select(mapper.Map<FighterHero>).ToList();


        await CreatePlannedMatchesAsync(mapper.Map<Tournament>(tournament), heroes);

        // temp solution for bo3 grand final
        if (tournament.CurrentStage == Stage.GrandFinals)
        {
            await CreatePlannedMatchesAsync(mapper.Map<Tournament>(tournament), heroes);
            await CreatePlannedMatchesAsync(mapper.Map<Tournament>(tournament), heroes);
        }
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
        await unitOfWork.Tournaments.AddOrUpdateAsync(tournament);
        
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
                    Map = maps.GetAndRemoveRandomItem(),
                    IsPlanned = true
                };
            generatedMatches.Add(match);
        }

        await UpdateAsync(tournament.Id, generatedMatches, tournament.CurrentStage);
    }
}
