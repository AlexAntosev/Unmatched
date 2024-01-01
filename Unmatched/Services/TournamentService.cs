namespace Unmatched.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Entities;
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
    
    public async Task<IEnumerable<TournamentDto>> GetAsync()
    {
        var entities = await _unitOfWork.Tournaments.Query().ToListAsync();
        var tournaments = _mapper.Map<IEnumerable<TournamentDto>>(entities);

        return tournaments;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _unitOfWork.Tournaments.Delete(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
