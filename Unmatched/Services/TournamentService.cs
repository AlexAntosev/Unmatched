namespace Unmatched.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Enums;
using Unmatched.Repositories;

public class TournamentService : ITournamentService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public TournamentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task AddAsync(TournamentDto dto)
    {
        var tournament = _mapper.Map<Tournament>(dto);
        await _unitOfWork.Tournaments.AddAsync(tournament);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, IEnumerable<MatchDto> matches, Stage stage)
    {
        var matchEntities = _mapper.Map<IEnumerable<Match>>(matches);

        foreach (var matchEntity in matchEntities)
        {
            var created = await _unitOfWork.Matches.AddAsync(matchEntity);
            
            var matchStage = new MatchStage
                {
                    MatchId = created.Id,
                    Stage = stage
                };
            await _unitOfWork.MatchStages.AddAsync(matchStage);
        }

        var tournament = await _unitOfWork.Tournaments.GetByIdAsync(id);
        
        tournament.CurrentStage = stage;
        _unitOfWork.Tournaments.AddOrUpdate(tournament);
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<TournamentDto>> GetAsync()
    {
        var entities = await _unitOfWork.Tournaments.Query().ToListAsync();
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
}
