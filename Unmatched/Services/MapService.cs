namespace Unmatched.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unmatched.Dtos;
using Unmatched.Repositories;

public class MapService : IMapService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MapService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<MapDto>> GetAsync()
    {
        var entities = await _unitOfWork.Maps.Query().OrderBy(x => x.Name).ToListAsync();
        var maps = _mapper.Map<IEnumerable<MapDto>>(entities);
        return maps;
    }
}
