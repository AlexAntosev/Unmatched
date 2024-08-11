namespace Unmatched.Services;

using System;

using AutoMapper;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Dtos;

public class PlayStyleService : IPlayStyleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlayStyleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task AddAsync(PlayStyleDto playStyleDto)
    {
        var playStyle = _mapper.Map<PlayStyle>(playStyleDto);
        await _unitOfWork.PlayStyles.AddAsync(playStyle);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Guid heroId, PlayStyleDto playStyleDto)
    {
        var entity = await _unitOfWork.PlayStyles.GetByHeroIdAsync(heroId);
        if (entity is not null)
        {
            entity.Attack = playStyleDto.Attack;
            entity.Defence = playStyleDto.Defence;
            entity.Trickery = playStyleDto.Trickery;
            entity.Difficulty = playStyleDto.Difficulty;
            _unitOfWork.PlayStyles.AddOrUpdate(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<PlayStyleDto> GetByHeroIdAsync(Guid heroId)
    {
        var entity = await _unitOfWork.PlayStyles.GetByHeroIdAsync(heroId);
        var playStyle = _mapper.Map<PlayStyleDto>(entity);

        return playStyle;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _unitOfWork.PlayStyles.Delete(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
