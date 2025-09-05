namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Dtos;

public class PlayStyleService : IPlayStyleService
{
    private readonly IMapper _mapper;

    private readonly IUnitOfWork _unitOfWork;

    public PlayStyleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddOrUpdateAsync(Guid heroId, PlayStyleDto playStyleDto)
    {
        //var entity = _mapper.Map<PlayStyle>(playStyleDto);
        //_unitOfWork.PlayStyles.AddOrUpdate(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
       // await _unitOfWork.PlayStyles.Delete(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PlayStyleDto> GetByHeroIdAsync(Guid heroId)
    {
        // var entity = await _unitOfWork.PlayStyles.GetByHeroIdAsync(heroId);
        //
        // var playStyle = entity != null
        //     ? _mapper.Map<PlayStyleDto>(entity)
        //     : PlayStyleDto.Default(heroId);
        //
        // return playStyle;
        return PlayStyleDto.Default(heroId);
    }
}
