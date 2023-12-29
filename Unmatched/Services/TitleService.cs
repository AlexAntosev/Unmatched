namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Entities;
using Unmatched.Repositories;

public class TitleService : ITitleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TitleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task AddAsync(TitleDto titleDto)
    {
        var title = _mapper.Map<Title>(titleDto);
        await _unitOfWork.Titles.AddAsync(title);
        await _unitOfWork.SaveChangesAsync();
    }
}
