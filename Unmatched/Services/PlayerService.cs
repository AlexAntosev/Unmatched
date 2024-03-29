﻿namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Dtos;

public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlayerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<PlayerDto>> GetAsync()
    {
        var entities = await _unitOfWork.Players.GetAsync();
        var players = _mapper.Map<IEnumerable<PlayerDto>>(entities);
        return players;
    }

    public async Task AddAsync(PlayerDto dto)
    {
        var player = _mapper.Map<Player>(dto);
        await _unitOfWork.Players.AddAsync(player);
        await _unitOfWork.SaveChangesAsync();
    }
}
